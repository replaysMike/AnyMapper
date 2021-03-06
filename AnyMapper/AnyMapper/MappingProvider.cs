using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using TypeSupport;
using TypeSupport.Extensions;

namespace AnyMapper
{
    /// <summary>
    /// AnyMapper mapping provider
    /// </summary>
    public class MappingProvider
    {
        public const int DefaultMaxDepth = 32;
        private readonly ObjectFactory _objectFactory;
        private readonly ICollection<object> _ignoreAttributes = new List<object> {
            typeof(IgnoreDataMemberAttribute),
            typeof(NonSerializedAttribute),
            "JsonIgnoreAttribute",
        };

        private MappingRegistry _typeRegistry;

        /// <summary>
        /// The type registry that contains mapping profiles
        /// </summary>
        public MappingRegistry TypeRegistry
        {
            get
            {
                if (_typeRegistry == null)
                    _typeRegistry = MappingConfigurationResolutionContext.GetMappingRegistry();
                return _typeRegistry;
            }
        }

        /// <summary>
        /// Create a provider for mapping objects
        /// </summary>
        public MappingProvider()
        {
            _objectFactory = new ObjectFactory();
        }

        /// <summary>
        /// Maps object to <typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="sourceObject"></param>
        /// <returns></returns>
        public TDest Map<TDest>(object sourceObject) => Map<object, TDest>(sourceObject, new List<string>());

        /// <summary>
        /// Maps <typeparamref name="TSource"/> to <typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="sourceObject"></param>
        /// <returns></returns>
        public TDest Map<TSource, TDest>(TSource sourceObject) => Map<TSource, TDest>(sourceObject, new List<string>());

        /// <summary>
        /// Maps <typeparamref name="TSource"/> to <typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="sourceObject"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public TDest Map<TSource, TDest>(TSource sourceObject, params Expression<Func<TSource, object>>[] ignoreProperties) 
            => Map<TSource, TDest>(sourceObject, ConvertToPropertyNameList(ignoreProperties));

        /// <summary>
        /// Maps <typeparamref name="TSource"/> to <typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="sourceObject"></param>
        /// <param name="ignorePropertiesOrPath"></param>
        /// <returns></returns>
        public TDest Map<TSource, TDest>(TSource sourceObject, ICollection<string> ignorePropertiesOrPath)
        {
            var type = typeof(TDest);
            var destExtendedType = type.GetExtendedType();
            var obj = InspectAndMap<TSource, TDest>(sourceObject, null, destExtendedType, 0, DefaultMaxDepth, MappingOptions.None, new Dictionary<ObjectHashcode, object>(), string.Empty, ignorePropertiesOrPath);

            // ChangeType doesn't like ICollection
            if (destExtendedType.IsCollection)
                return (TDest)obj;

            return (TDest)Convert.ChangeType(obj, type);
        }

        /// <summary>
        /// Maps <typeparamref name="TSource"/> to <typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="sourceObject"></param>
        /// <param name="destObject"></param>
        /// <returns></returns>
        public TDest Map<TSource, TDest>(TSource sourceObject, TDest destObject) 
            => Map(sourceObject, destObject, new List<string>());

        /// <summary>
        /// Maps <typeparamref name="TSource"/> to <typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="sourceObject"></param>
        /// <param name="destObject"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public TDest Map<TSource, TDest>(TSource sourceObject, TDest destObject, params Expression<Func<TSource, object>>[] ignoreProperties) 
            => Map(sourceObject, destObject, ConvertToPropertyNameList(ignoreProperties));

        /// <summary>
        /// Maps <typeparamref name="TSource"/> to <typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="sourceObject"></param>
        /// <param name="destObject"></param>
        /// <param name="ignorePropertiesOrPaths"></param>
        /// <returns></returns>
        public TDest Map<TSource, TDest>(TSource sourceObject, TDest destObject, ICollection<string> ignorePropertiesOrPaths)
        {
            var type = typeof(TDest);
            var destExtendedType = type.GetExtendedType();
            var obj = InspectAndMap<TSource, TDest>(sourceObject, destObject, destExtendedType, 0, DefaultMaxDepth, MappingOptions.None, new Dictionary<ObjectHashcode, object>(), string.Empty, ignorePropertiesOrPaths);

            // ChangeType doesn't like ICollection
            if (destExtendedType.IsCollection)
                return (TDest)obj;
            return (TDest)Convert.ChangeType(obj, type);
        }

        /// <summary>
        /// (Recursive) Recursive function that inspects an object and its properties/fields and clones it
        /// </summary>
        /// <param name="sourceObject">The object to clone</param>
        /// <param name="destObject">The destination object</param>
        /// <param name="mapToType">The type to map to</param>
        /// <param name="currentDepth">The current tree depth</param>
        /// <param name="maxDepth">The max tree depth</param>
        /// <param name="options">The cloning options</param>
        /// <param name="objectTree">The object tree to prevent cyclical references</param>
        /// <param name="path">The current path being traversed</param>
        /// <param name="ignorePropertiesOrPaths">A list of properties or paths to ignore</param>
        /// <returns></returns>
        private object InspectAndMap<TSource, TDest>(object sourceObject, object destObject, ExtendedType mapToType, int currentDepth, int maxDepth, MappingOptions options, IDictionary<ObjectHashcode, object> objectTree, string path, ICollection<string> ignorePropertiesOrPaths = null)
        {
            if (IgnoreObjectName(null, path, options, ignorePropertiesOrPaths))
                return null;

            if (sourceObject == null)
                return null;

            // ensure we don't go too deep if specified
            if (maxDepth > 0 && currentDepth >= maxDepth)
                return null;

            var sourceType = typeof(TSource).GetExtendedType();
            var destType = typeof(TDest).GetExtendedType();

            if (ignorePropertiesOrPaths == null)
                ignorePropertiesOrPaths = new List<string>();

            // drop any objects we are ignoring by attribute
            if (mapToType.Attributes.Any(x => _ignoreAttributes.Contains(x)) && options.BitwiseHasFlag(MappingOptions.DisableIgnoreAttributes))
                return null;

            // for delegate types, copy them by reference rather than returning null
            if (mapToType.IsDelegate)
                return sourceObject;

            object newObject = destObject;
            // create a new empty object of the desired type
            if (newObject == null)
            {
                if (mapToType.IsArray)
                {
                    var length = 0;
                    if (mapToType.IsArray)
                        length = (sourceObject as Array).Length;
                    newObject = _objectFactory.CreateEmptyObject(mapToType.Type, length);
                }
                else if (mapToType.Type == typeof(string))
                {
                    // copy the item directly
                    newObject = Convert.ToString(sourceObject);
                    return newObject;
                }
                else
                {
                    newObject = _objectFactory.CreateEmptyObject(mapToType.Type);
                }
            }

            if (newObject == null)
                return newObject;

            // increment the current recursion depth
            currentDepth++;

            // construct a hashtable of objects we have already inspected (simple recursion loop preventer)
            // we use this hashcode method as it does not use any custom hashcode handlers the object might implement
            if (sourceObject != null && !mapToType.IsValueType)
            {
                var hashCode = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(sourceObject);
                var key = new ObjectHashcode(hashCode, newObject.GetType());
                if (objectTree.ContainsKey(key))
                    return objectTree[key];

                // ensure we can refer back to the reference for this object
                objectTree.Add(key, newObject);
            }

            var objectMapper = TypeRegistry.ObjectMappings
                .FirstOrDefault(x => x.SourceObjectType == sourceType.Type
                    && x.DestinationObjectType == destType.Type);

            try
            {
                // clone a dictionary's key/values
                if (mapToType.IsDictionary && mapToType.IsGeneric)
                {
                    var genericType = mapToType.Type.GetGenericArguments().ToList();
                    Type[] typeArgs = { genericType[0], genericType[1] };

                    var listType = typeof(Dictionary<,>).MakeGenericType(typeArgs);
                    var newDictionary = Activator.CreateInstance(listType) as IDictionary;
                    newObject = newDictionary;
                    var enumerator = (IDictionary)sourceObject;
                    foreach (DictionaryEntry item in enumerator)
                    {
                        var key = InspectAndMap<TSource, TDest>(item.Key, null, item.Key.GetExtendedType(), currentDepth, maxDepth, options, objectTree, path, ignorePropertiesOrPaths);
                        var value = InspectAndMap<TSource, TDest>(item.Value, null, item.Value.GetExtendedType(), currentDepth, maxDepth, options, objectTree, path, ignorePropertiesOrPaths);
                        newDictionary.Add(key, value);
                    }
                    return newObject;
                }

                // clone an enumerables' elements
                if (mapToType.IsEnumerable && mapToType.IsGeneric)
                {
                    var genericType = mapToType.Type.GetGenericArguments().First();
                    var genericExtendedType = genericType.GetExtendedType();
                    var addMethod = mapToType.Type.GetMethod("Add");
                    var enumerator = (IEnumerable)sourceObject;
                    foreach (var item in enumerator)
                    {
                        var element = InspectAndMap<TSource, TDest>(item, null, genericExtendedType, currentDepth, maxDepth, options, objectTree, path, ignorePropertiesOrPaths);
                        addMethod.Invoke(newObject, new object[] { element });
                    }
                    return newObject;
                }

                // clone an arrays' elements
                if (mapToType.IsArray)
                {
                    var sourceArray = sourceObject as Array;
                    var newArray = newObject as Array;
                    newObject = newArray;
                    for (var i = 0; i < sourceArray.Length; i++)
                    {
                        var element = sourceArray.GetValue(i);
                        var newElement = InspectAndMap<TSource, TDest>(element, null, mapToType.ElementType.GetExtendedType(), currentDepth, maxDepth, options, objectTree, path, ignorePropertiesOrPaths);
                        newArray.SetValue(newElement, i);
                    }
                    return newArray;
                }

                var fields = sourceObject.GetFields(FieldOptions.AllWritable);
                var properties = sourceObject.GetProperties(PropertyOptions.HasGetter);

                var rootPath = path;
                // clone and recurse fields
                if (newObject != null)
                {
                    foreach (var field in fields)
                    {
                        path = $"{rootPath}.{field.Name}";
                        if (IgnoreObjectName(field.Name, path, options, ignorePropertiesOrPaths, field.CustomAttributes))
                            continue;
                        // also check the property for ignore, if this is a auto-backing property
                        if (field.BackedProperty != null && IgnoreObjectName(field.BackedProperty.Name, $"{rootPath}.{field.BackedPropertyName}", options, ignorePropertiesOrPaths, field.BackedProperty.CustomAttributes))
                            continue;
                        newObject = MapField<TSource, TDest>(newObject, sourceObject, objectMapper, field, currentDepth, maxDepth, options, objectTree, path, ignorePropertiesOrPaths);
                    }
                    foreach (var property in properties)
                    {
                        path = $"{rootPath}.{property.Name}";
                        if (IgnoreObjectName(property.Name, path, options, ignorePropertiesOrPaths, property.CustomAttributes))
                            continue;

                        // also check the backing field for ignore, if this is a auto-backing property
                        if (property.BackingFieldName != null && IgnoreObjectName(property.BackingFieldName, $"{rootPath}.{property.BackingFieldName}", options, ignorePropertiesOrPaths, fields.FirstOrDefault(x => x.Name == property.BackingFieldName).CustomAttributes))
                            continue;

                        if (string.IsNullOrEmpty(property.BackingFieldName))
                        {
                            // map the property, it has no backing field so it's likely a method call
                            newObject = MapProperty<TSource, TDest>(newObject, sourceObject, objectMapper, property, currentDepth, maxDepth, options, objectTree, path, ignorePropertiesOrPaths);
                        }
                    }
                }

                return newObject;
            }
            finally
            {

            }
        }

        private object MapField<TSource, TDest>(object newObject, object sourceObject, ObjectMap objectMapper, ExtendedField field, int currentDepth, int maxDepth, MappingOptions options, IDictionary<ObjectHashcode, object> objectTree, string path, ICollection<string> ignorePropertiesOrPaths = null)
        {
            var sourceFieldName = field.Name;
            var sourceFieldBackedPropertyName = field.BackedPropertyName;
            var sourceFieldType = field.Type;
            var sourceField = new Field(sourceFieldBackedPropertyName ?? sourceFieldName, sourceFieldType, field.ReflectedType);
            var sourceFieldValue = sourceObject.GetFieldValue(field);

            var destinationFieldName = field.Name;
            var destinationFieldBackedPropertyName = field.BackedPropertyName;
            var destinationFieldType = sourceFieldType;
            var destinationField = new Field(destinationFieldBackedPropertyName, destinationFieldType, field.ReflectedType);
            // determine from the registry what we are mapping this to
            var fieldMapper = objectMapper?.Mappings
                .Where(x =>
                    x.Source.DeclaringType == sourceFieldType
                    && x.Source.Name == field.Name || (field.IsBackingField && x.Source.Name == field.BackedPropertyName))
                .FirstOrDefault();
            if (fieldMapper != null)
            {
                destinationFieldName = fieldMapper.Destination.Name;
                destinationFieldType = fieldMapper.Destination.Type;
                destinationFieldBackedPropertyName = fieldMapper.Destination.Name;
                destinationField = new Field(destinationFieldName, destinationFieldType, fieldMapper.Destination.DeclaringType);
            }

            FieldInfo destinationFieldInfo = null;
            PropertyInfo destinationPropertyInfo = null;
            destinationFieldInfo = newObject.GetField(destinationFieldName, sourceFieldType.Type);
            if (destinationFieldInfo == null)
            {
                if (sourceFieldType.IsNullable)
                {
                    // support nullable => non-nullable
                    destinationFieldInfo = newObject.GetField(destinationFieldName, sourceFieldType.NullableBaseType);
                    if (destinationFieldInfo != null)
                        destinationFieldType = sourceFieldType.NullableBaseType.GetExtendedType();
                }
            }
            if (destinationFieldInfo == null)
            {
                // doesn't exist on the other side, try getting its property
                // todo: reduce the code duplication here
                if (field.IsBackingField)
                {
                    destinationPropertyInfo = newObject.GetProperty(destinationFieldBackedPropertyName, sourceFieldType.Type);
                    if (destinationPropertyInfo == null) {
                        // support non-nullable => nullable
                        destinationPropertyInfo = newObject.GetProperty(destinationFieldBackedPropertyName, GetNullableType(sourceFieldType.Type));
                        if (destinationPropertyInfo != null)
                            destinationFieldType = GetNullableType(sourceFieldType.Type).GetExtendedType();
                    }
                }
                else
                {
                    destinationPropertyInfo = newObject.GetProperty(destinationFieldName, sourceFieldType.Type);
                    if (destinationPropertyInfo == null) {
                        // support non-nullable => nullable
                        destinationPropertyInfo = newObject.GetProperty(destinationFieldName, GetNullableType(sourceFieldType.Type));
                        if (destinationPropertyInfo != null)
                            destinationFieldType = GetNullableType(sourceFieldType.Type).GetExtendedType();
                    }
                }
            }

            if (destinationFieldInfo != null || destinationPropertyInfo != null)
            {
                var sourceBaseType = sourceFieldType.IsNullable ? sourceFieldType.NullableBaseType : sourceFieldType.Type;
                var destinationBaseType = destinationFieldType.IsNullable ? destinationFieldType.NullableBaseType : destinationFieldType.Type;
                if (sourceBaseType != destinationBaseType)
                    throw new MappingException(sourceField, destinationField);

                if (sourceFieldType.IsValueType || sourceFieldType.IsImmutable)
                {
                    if (destinationFieldInfo != null)
                        newObject.SetFieldValue(destinationFieldName, destinationFieldType.Type, sourceFieldValue);
                    else
                    {
                        if (field.IsBackingField)
                            newObject.SetPropertyValue(destinationFieldBackedPropertyName, destinationFieldType.Type, sourceFieldValue);
                        else
                            newObject.SetPropertyValue(destinationFieldName, destinationFieldType.Type, sourceFieldValue);
                    }
                }
                else if (sourceFieldValue != null)
                {
                    var clonedFieldValue = InspectAndMap<TSource, TDest>(sourceFieldValue, null, sourceFieldType, currentDepth, maxDepth, options, objectTree, path, ignorePropertiesOrPaths);
                    if (destinationFieldInfo != null)
                        newObject.SetFieldValue(destinationFieldName, destinationFieldType.Type, clonedFieldValue);
                    else
                    {
                        if (field.IsBackingField)
                            newObject.SetPropertyValue(destinationFieldBackedPropertyName, destinationFieldType.Type, clonedFieldValue);
                        else
                            newObject.SetPropertyValue(destinationFieldName, destinationFieldType.Type, clonedFieldValue);
                    }
                }
            }
            else
            {
                // destination field does not exist or is not mapped
                // throw new MappingException($"There is no mapping configured for the property {MappingException.FormatField(sourceField)}");
            }
            return newObject;
        }

        private object MapProperty<TSource, TDest>(object newObject, object sourceObject, ObjectMap objectMapper, ExtendedProperty property, int currentDepth, int maxDepth, MappingOptions options, IDictionary<ObjectHashcode, object> objectTree, string path, ICollection<string> ignorePropertiesOrPaths = null)
        {
            var sourcePropertyName = property.Name;
            var sourcePropertyType = property.Type;
            var sourceField = new Field(sourcePropertyName, sourcePropertyType, property.ReflectedType);
            var sourceFieldValue = sourceObject.GetPropertyValue(property);

            var destinationFieldName = property.Name;
            var destinationFieldType = sourcePropertyType;
            var destinationField = new Field(destinationFieldName, destinationFieldType, property.ReflectedType);
            // determine from the registry what we are mapping this to
            var fieldMapper = objectMapper?.Mappings
                .Where(x =>
                    x.Source.DeclaringType == sourcePropertyType
                    && x.Source.Name == property.Name)
                .FirstOrDefault();
            if (fieldMapper != null)
            {
                destinationFieldName = fieldMapper.Destination.Name;
                destinationFieldType = fieldMapper.Destination.Type;
                destinationField = new Field(destinationFieldName, destinationFieldType, fieldMapper.Destination.DeclaringType);
            }

            FieldInfo destinationFieldInfo = null;
            PropertyInfo destinationPropertyInfo = null;
            destinationFieldInfo = newObject.GetField(destinationFieldName, property.Type);
            if (destinationFieldInfo == null)
            {
                // doesn't exist on the other side, try getting its property
                destinationPropertyInfo = newObject.GetProperty(destinationFieldName, property.Type);
            }

            if (destinationPropertyInfo != null)
            {
                if (destinationFieldInfo?.FieldType != sourcePropertyType.Type
                    && destinationPropertyInfo?.PropertyType != sourcePropertyType.Type)
                    throw new MappingException(sourceField, destinationField);

                if (sourcePropertyType.IsValueType || sourcePropertyType.IsImmutable)
                {
                    try
                    {
                        newObject.SetPropertyValue(destinationFieldName, property.Type, sourceFieldValue);
                    }
                    catch (Exception) { }
                }
                else if (sourceFieldValue != null)
                {
                    var clonedFieldValue = InspectAndMap<TSource, TDest>(sourceFieldValue, null, sourcePropertyType, currentDepth, maxDepth, options, objectTree, path, ignorePropertiesOrPaths);
                    try
                    {
                        newObject.SetPropertyValue(destinationFieldName, property.Type, clonedFieldValue);
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                // destination field does not exist or is not mapped
                // throw new MappingException($"There is no mapping configured for the property {MappingException.FormatField(sourceField)}");
            }
            return newObject;
        }

        /// <summary>
        /// Returns true if object name should be ignored
        /// </summary>
        /// <param name="name">Property or field name</param>
        /// <param name="path">Full path to object</param>
        /// <param name="options">Comparison options</param>
        /// <param name="ignorePropertiesOrPaths">List of names or paths to ignore</param>
        /// <returns></returns>
        private bool IgnoreObjectName(string name, string path, MappingOptions options, ICollection<string> ignorePropertiesOrPaths, IEnumerable<CustomAttributeData> attributes = null)
        {
            var ignoreByNameOrPath = ignorePropertiesOrPaths?.Contains(name) == true || ignorePropertiesOrPaths?.Contains(path) == true;
            if (ignoreByNameOrPath)
                return true;
#if FEATURE_CUSTOM_ATTRIBUTES
            if (attributes?.Any(x => !options.BitwiseHasFlag(MappingOptions.DisableIgnoreAttributes) && (_ignoreAttributes.Contains(x.AttributeType) || _ignoreAttributes.Contains(x.AttributeType.Name))) == true)
#else
            if (attributes?.Any(x => !options.BitwiseHasFlag(MappingOptions.DisableIgnoreAttributes) && (_ignoreAttributes.Contains(x.Constructor.DeclaringType) || _ignoreAttributes.Contains(x.Constructor.DeclaringType.Name))) == true)
#endif
                return true;
            return false;
        }

        /// <summary>
        /// Convert an expression of properties to a list of property names
        /// </summary>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        private ICollection<string> ConvertToPropertyNameList<TSource>(Expression<Func<TSource, object>>[] ignoreProperties)
        {
            var ignorePropertiesList = new List<string>();
            foreach (var expression in ignoreProperties)
            {
                var name = "";
                switch (expression.Body)
                {
                    case MemberExpression m:
                        name = m.Member.Name;
                        break;
                    case UnaryExpression u when u.Operand is MemberExpression m:
                        name = m.Member.Name;
                        break;
                    default:
                        throw new NotImplementedException(expression.GetType().ToString());
                }
                ignorePropertiesList.Add(name);
            }
            return ignorePropertiesList;
        }

        private Type GetNullableType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type.IsValueType)
                return typeof(Nullable<>).MakeGenericType(type);
            else
                return type;
        }
    }

    public struct ObjectHashcode
    {
        public int Hashcode { get; set; }
        public Type Type { get; set; }
        public ObjectHashcode(int hashcode, Type type)
        {
            Hashcode = hashcode;
            Type = type;
        }

        public override int GetHashCode()
        {
            var computedHashcode = 23;
            computedHashcode = computedHashcode * 31 + Hashcode;
            computedHashcode = computedHashcode * 31 + Type.GetHashCode();
            return computedHashcode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var typedObj = (ObjectHashcode)obj;
            return typedObj.Hashcode.Equals(Hashcode) && typedObj.Type.Equals(Type);
        }
    }
}
