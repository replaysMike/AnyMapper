using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using TypeSupport;
using TypeSupport.Extensions;

namespace AnyMapper
{
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
        /// todo: a singleton instance isn't ideal
        /// </summary>
        public MappingRegistry TypeRegistry
        {
            get
            {
                if(_typeRegistry == null)
                    _typeRegistry = MappingConfigurationResolutionContext.GetMappingRegistry();
                return _typeRegistry;
            }
        }

        /// <summary>
        /// Provider for cloning objects
        /// </summary>
        public MappingProvider()
        {
            _objectFactory = new ObjectFactory();
        }

        public TDest Map<TSource, TDest>(TSource sourceObject, MappingContext context, MapOptions options)
        {
            var obj = InspectAndMap<TSource, TDest>(sourceObject, null, typeof(TDest).GetExtendedType(), 0, DefaultMaxDepth, options, MappingSetupOptions.None, new Dictionary<int, object>(), string.Empty);
            return (TDest)Convert.ChangeType(obj, typeof(TDest));
        }

        public TDest Map<TSource, TDest>(TSource sourceObject, MappingContext context, MapOptions options, params Expression<Func<TSource, object>>[] ignoreProperties)
        {
            var obj = InspectAndMap<TSource, TDest>(sourceObject, null, typeof(TDest).GetExtendedType(), 0, DefaultMaxDepth, options, MappingSetupOptions.None, new Dictionary<int, object>(), string.Empty, ConvertToPropertyNameList(ignoreProperties));
            return (TDest)Convert.ChangeType(obj, typeof(TDest));
        }

        public TDest Map<TSource, TDest>(TSource sourceObject, MappingContext context, MapOptions options, params string[] ignorePropertiesOrPaths)
        {
            var obj = InspectAndMap<TSource, TDest>(sourceObject, null, typeof(TDest).GetExtendedType(), 0, DefaultMaxDepth, options, MappingSetupOptions.None, new Dictionary<int, object>(), string.Empty, ignorePropertiesOrPaths);
            return (TDest)Convert.ChangeType(obj, typeof(TDest));
        }

        public TDest Map<TSource, TDest>(TSource sourceObject, TDest destObject, MappingContext context, MapOptions options)
        {
            var obj = InspectAndMap<TSource, TDest>(sourceObject, destObject, typeof(TDest).GetExtendedType(), 0, DefaultMaxDepth, options, MappingSetupOptions.None, new Dictionary<int, object>(), string.Empty);
            return (TDest)Convert.ChangeType(obj, typeof(TDest));
        }

        public TDest Map<TSource, TDest>(TSource sourceObject, TDest destObject, MappingContext context, MapOptions options, params Expression<Func<TSource, object>>[] ignoreProperties)
        {
            var obj = InspectAndMap<TSource, TDest>(sourceObject, destObject, typeof(TDest).GetExtendedType(), 0, DefaultMaxDepth, options, MappingSetupOptions.None, new Dictionary<int, object>(), string.Empty, ConvertToPropertyNameList(ignoreProperties));
            return (TDest)Convert.ChangeType(obj, typeof(TDest));
        }

        public TDest Map<TSource, TDest>(TSource sourceObject, TDest destObject, MappingContext context, MapOptions options, params string[] ignorePropertiesOrPaths)
        {
            var obj = InspectAndMap<TSource, TDest>(sourceObject, destObject, typeof(TDest).GetExtendedType(), 0, DefaultMaxDepth, options, MappingSetupOptions.None, new Dictionary<int, object>(), string.Empty, ignorePropertiesOrPaths);
            return (TDest)Convert.ChangeType(obj, typeof(TDest));
        }

        /// <summary>
        /// (Recursive) Recursive function that inspects an object and its properties/fields and clones it
        /// </summary>
        /// <param name="sourceObject">The object to clone</param>
        /// <param name="currentDepth">The current tree depth</param>
        /// <param name="maxDepth">The max tree depth</param>
        /// <param name="setupOptions">The cloning options</param>
        /// <param name="objectTree">The object tree to prevent cyclical references</param>
        /// <param name="path">The current path being traversed</param>
        /// <returns></returns>
        private object InspectAndMap<TSource, TDest>(object sourceObject, object destObject, ExtendedType mapToType, int currentDepth, int maxDepth, MapOptions mapOptions, MappingSetupOptions setupOptions, IDictionary<int, object> objectTree, string path, ICollection<string> ignorePropertiesOrPaths = null)
        {
            if (IgnoreObjectName(null, path, mapOptions, setupOptions, ignorePropertiesOrPaths))
                return null;

            if (sourceObject == null)
                return null;

            // ensure we don't go too deep if specified
            if (maxDepth > 0 && currentDepth >= maxDepth)
                return null;

            var sourceType = new ExtendedType(typeof(TSource));
            var destType = new ExtendedType(typeof(TDest));

            if (ignorePropertiesOrPaths == null)
                ignorePropertiesOrPaths = new List<string>();

            // drop any objects we are ignoring by attribute
            if (mapToType.Attributes.Any(x => _ignoreAttributes.Contains(x)) && setupOptions.BitwiseHasFlag(MappingSetupOptions.DisableIgnoreAttributes))
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
                    newObject = _objectFactory.CreateEmptyObject(mapToType.Type, length: length);
                }
                else if (mapToType.Type == typeof(string))
                {
                    // copy the item directly
                    newObject = String.Copy(Convert.ToString(sourceObject));
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
                if (objectTree.ContainsKey(hashCode))
                    return objectTree[hashCode];

                // ensure we can refer back to the reference for this object
                objectTree.Add(hashCode, newObject);
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
                        var key = InspectAndMap<TSource, TDest>(item.Key, null, item.Key.GetExtendedType(), currentDepth, maxDepth, mapOptions, setupOptions, objectTree, path, ignorePropertiesOrPaths);
                        var value = InspectAndMap<TSource, TDest>(item.Value, null, item.Value.GetExtendedType(), currentDepth, maxDepth, mapOptions, setupOptions, objectTree, path, ignorePropertiesOrPaths);
                        newDictionary.Add(key, value);
                    }
                    return newObject;
                }

                // clone an enumerables' elements
                if (mapToType.IsEnumerable && mapToType.IsGeneric)
                {
                    var genericType = mapToType.Type.GetGenericArguments().First();
                    var genericExtendedType = new ExtendedType(genericType);
                    var addMethod = mapToType.Type.GetMethod("Add");
                    var enumerator = (IEnumerable)sourceObject;
                    foreach (var item in enumerator)
                    {
                        var element = InspectAndMap<TSource, TDest>(item, null, item.GetExtendedType(), currentDepth, maxDepth, mapOptions, setupOptions, objectTree, path, ignorePropertiesOrPaths);
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
                        var newElement = InspectAndMap<TSource, TDest>(element, null, element.GetExtendedType(), currentDepth, maxDepth, mapOptions, setupOptions, objectTree, path, ignorePropertiesOrPaths);
                        newArray.SetValue(newElement, i);
                    }
                    return newArray;
                }

                var fields = sourceObject.GetFields(FieldOptions.AllWritable);

                var rootPath = path;
                // clone and recurse fields
                if (newObject != null)
                {
                    foreach (var field in fields)
                    {
                        path = $"{rootPath}.{field.Name}";
                        // check the source field for ignore
                        if (IgnoreObjectName(field.Name, path, mapOptions, setupOptions, ignorePropertiesOrPaths, field.CustomAttributes))
                            continue;
                        // also check the backed source property for ignore, if this is a auto-backing property
                        if (field.BackedProperty != null && IgnoreObjectName(field.BackedProperty.Name, $"{rootPath}.{field.BackedPropertyName}", mapOptions, setupOptions, ignorePropertiesOrPaths, field.BackedProperty.CustomAttributes))
                            continue;

                        var sourceFieldName = field.Name;
                        var sourceFieldBackedPropertyName = field.BackedPropertyName;
                        var sourceFieldType = new ExtendedType(field.Type);
                        var sourceField = new Field(sourceFieldBackedPropertyName ?? sourceFieldName, sourceFieldType, field.ReflectedType.GetExtendedType());
                        var sourceFieldValue = sourceObject.GetFieldValue(field);

                        var destinationFieldName = field.Name;
                        var destinationFieldBackedPropertyName = field.BackedPropertyName;
                        var destinationFieldType = sourceFieldType;
                        var destinationField = new Field(destinationFieldBackedPropertyName, destinationFieldType, field.ReflectedType.GetExtendedType());
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
                            destinationField = new Field(destinationFieldName, destinationFieldType, fieldMapper.Destination.DeclaringType);
                        }

                        ExtendedField destinationFieldInfo = newObject.GetField(destinationFieldName);
                        ExtendedProperty destinationPropertyInfo = null;
                        if (destinationFieldInfo == null)
                        {
                            // doesn't exist on the other side
                            destinationPropertyInfo = newObject.GetProperty(destinationFieldName);
                        }

                        // also check the destination field for ignore
                        if (destinationFieldInfo != null && IgnoreObjectName(destinationFieldInfo.Name, path, mapOptions, setupOptions, ignorePropertiesOrPaths, destinationFieldInfo.CustomAttributes))
                            continue;
                        // also check the backed destination property for ignore, if this is a auto-backing property
                        if (destinationFieldInfo?.BackedProperty != null && IgnoreObjectName(destinationFieldInfo.BackedProperty.Name, $"{rootPath}.{destinationFieldInfo.BackedPropertyName}", mapOptions, setupOptions, ignorePropertiesOrPaths, destinationFieldInfo.BackedProperty.CustomAttributes))
                            continue;
                        // also check the destination property for ignore
                        if (destinationPropertyInfo != null && IgnoreObjectName(destinationPropertyInfo.Name, path, mapOptions, setupOptions, ignorePropertiesOrPaths, destinationPropertyInfo.CustomAttributes))
                            continue;

                        if (destinationFieldInfo != null || destinationPropertyInfo != null)
                        {
                            if (destinationFieldInfo?.Type != sourceFieldType.Type
                                && destinationPropertyInfo?.Type != sourceFieldType.Type)
                                throw new MappingException(sourceField, destinationField);

                            if (sourceFieldType.IsValueType || sourceFieldType.IsImmutable)
                            {
                                if (destinationFieldInfo != null)
                                    newObject.SetFieldValue(destinationFieldName, sourceFieldValue);
                                else
                                    newObject.SetPropertyValue(destinationFieldName, sourceFieldValue);
                            }
                            else if (sourceFieldValue != null)
                            {
                                var clonedFieldValue = InspectAndMap<TSource, TDest>(sourceFieldValue, null, sourceFieldType, currentDepth, maxDepth, mapOptions, setupOptions, objectTree, path, ignorePropertiesOrPaths);
                                if (destinationFieldInfo != null)
                                    newObject.SetFieldValue(destinationFieldName, clonedFieldValue);
                                else
                                    newObject.SetPropertyValue(destinationFieldName, clonedFieldValue);
                            }
                        }
                        else
                        {
                            // destination field does not exist or is not mapped
                            // throw new MappingException($"There is no mapping configured for the property {MappingException.FormatField(sourceField)}");
                        }
                    }
                }

                return newObject;
            }
            finally
            {

            }
        }

        /// <summary>
        /// Returns true if object name should be ignored
        /// </summary>
        /// <param name="name">Property or field name</param>
        /// <param name="path">Full path to object</param>
        /// <param name="setupOptions">Comparison options</param>
        /// <param name="ignorePropertiesOrPaths">List of names or paths to ignore</param>
        /// <returns></returns>
        private bool IgnoreObjectName(string name, string path, MapOptions mapOptions, MappingSetupOptions setupOptions, ICollection<string> ignorePropertiesOrPaths, IEnumerable<CustomAttributeData> attributes = null)
        {
            var ignoreByNameOrPath = ignorePropertiesOrPaths?.Contains(name) == true || ignorePropertiesOrPaths?.Contains(path) == true;
            if (ignoreByNameOrPath)
                return true;
#if FEATURE_CUSTOM_ATTRIBUTES
            if (attributes?.Any(x => !setupOptions.BitwiseHasFlag(MappingSetupOptions.DisableIgnoreAttributes) 
                    && (_ignoreAttributes.Contains(x.AttributeType) || _ignoreAttributes.Contains(x.AttributeType.Name))) == true 
               || attributes?.Any(x => mapOptions.BitwiseHasFlag(MapOptions.IgnoreEntityKeys)
                    && (x.AttributeType.Name == "KeyAttribute")) == true
                || attributes?.Any(x => mapOptions.BitwiseHasFlag(MapOptions.IgnoreEntityAutoIncrementProperties)
                    && (x.AttributeType.Name == "DatabaseGeneratedAttribute" && x.ConstructorArguments.Any() && (int)x.ConstructorArguments.First().Value == (int)DatabaseGeneratedOption.Identity)) == true
            )
#else
            if (attributes?.Any(x => !options.BitwiseHasFlag(MappingSetupOptions.DisableIgnoreAttributes) 
                    && (_ignoreAttributes.Contains(x.Constructor.DeclaringType) || _ignoreAttributes.Contains(x.Constructor.DeclaringType.Name))) == true)
                || attributes?.Any(x => mapOptions.BitwiseHasFlag(MapOptions.IgnoreEntityKeys)
                    && (x.Constructor.DeclaringType.Name == "KeyAttribute")) == true
                || attributes?.Any(x => mapOptions.BitwiseHasFlag(MapOptions.IgnoreEntityAutoIncrementProperties)
                    && (x.Constructor.DeclaringType.Name == "DatabaseGeneratedAttribute" && x.ConstructorArguments.Any() && (int)x.ConstructorArguments.First().Value == (int)DatabaseGeneratedOption.Identity)) == true

            )
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
    }
}
