using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using TypeSupport;
using TypeSupport.Extensions;

namespace AnyMapper
{
    public class MappingRegistry
    {
        public ICollection<FieldMap> Mappings { get; private set; }
        public ICollection<ObjectMap> ObjectMappings { get; private set; }

        public MappingRegistry()
        {
            Mappings = new List<FieldMap>();
            ObjectMappings = new List<ObjectMap>();
        }

        public MappingRegistry(params FieldMap[] mappings)
        {
            var mappingsList = new List<FieldMap>();
            mappingsList.AddRange(mappings);
            Mappings = mappingsList;
        }

        public MappingRegistry(params Profile[] profiles)
        {
            var mappingsList = new List<FieldMap>();
            foreach (var profile in profiles)
            {
                var mappings = Mappings.Where(x => x.ProfileType == profile.GetType()).ToList();
                mappingsList.AddRange(mappings);
            }
            Mappings = mappingsList;
        }

        public MappingRegistry(bool isRegistered, params Profile[] profiles)
        {
            var mappingsList = new List<FieldMap>();
            foreach (var profile in profiles)
            {
                var mappings = Mappings.Where(x => x.ProfileType == profile.GetType()).ToList();
                mappings.ForEach(x => { x.IsRegistered = isRegistered; });
                mappingsList.AddRange(mappings);
            }
            Mappings = mappingsList;
        }

        public void AddMapping(FieldMap map)
        {
            AddMapping(map, map.IsRegistered);
        }

        public void AddMapping(FieldMap map, bool isRegistered)
        {
            var sourceObjectType = map.Source.DeclaringType.Type;
            var destinationObjectType = map.Destination.DeclaringType.Type;
            var objectMap = ObjectMappings.FirstOrDefault(x => x.SourceObjectType == sourceObjectType 
                && x.DestinationObjectType == destinationObjectType);

            if (objectMap == null)
            {
                objectMap = new ObjectMap(map);
                ObjectMappings.Add(objectMap);
            }

            var existingMapping = Mappings.Where(x => x == map).FirstOrDefault();
            if (existingMapping == null)
            {
                map.IsRegistered = isRegistered;
                objectMap.Add(map);
                Mappings.Add(map);
            }
            else
            {
                existingMapping.IsRegistered = isRegistered;
            }
        }

        public void AddMapping(bool isRegistered, params Profile[] profiles)
        {
            foreach (var profile in profiles)
            {
                var mappings = Mappings.Where(x => x.ProfileType == profile.GetType()).ToList();
                foreach (var mapping in mappings)
                    AddMapping(mapping, isRegistered);
            }
        }

        public void AddMapping<TSource, TDest>(MappingExpression<TSource, TDest> map)
        {
            AddMapping(map, false);
        }

        public void AddMapping<TSource, TDest>(MappingExpression<TSource, TDest> map, bool isRegistered)
        {
            var mapping = new FieldMap(map.ProfileType, GetFieldFromExpression(map.Source, isRegistered), GetFieldFromExpression(map.Destination, isRegistered), isRegistered);
            AddMapping(mapping, isRegistered);
        }

        public Field GetFieldFromExpression<T>(Expression<Func<T, object>> expression, bool isRegistered)
        {
            var name = string.Empty;
            ExtendedType type = null;
            ExtendedType declaringType = null;
            var mm = expression.Body as UnaryExpression;
            var mt = expression.Body.GetType();
            var oo = mm.Operand.GetType();

            switch (expression.Body)
            {
                case MemberExpression m:
                    name = m.Member.Name;
                    if (m.Member.MemberType == System.Reflection.MemberTypes.Property)
                        type = ((PropertyInfo)m.Member).PropertyType.GetExtendedType();
                    declaringType = m.Member.DeclaringType.GetExtendedType();
                    break;
                case UnaryExpression u when u.Operand is MemberExpression m:
                    name = m.Member.Name;
                    if(m.Member.MemberType == System.Reflection.MemberTypes.Property)
                        type = ((PropertyInfo)m.Member).PropertyType.GetExtendedType();
                    declaringType = m.Member.DeclaringType.GetExtendedType();
                    break;
                case UnaryExpression u:
                    name = "";
                    type = u.Operand.Type.GetExtendedType();
                    declaringType = u.Operand.Type.GetExtendedType();
                    break;
                default:
                    throw new NotImplementedException(expression.GetType().ToString());
            }
            return new Field(name, type, declaringType, isRegistered);
        }

        public override string ToString()
        {
            return $"{Mappings.Count(x => x.IsRegistered)} mappings registered, {Mappings.Count(x => !x.IsRegistered)} ambient registrations";
        }
    }

    public class ObjectMap
    {
        public Type ProfileType { get; }
        public Type SourceObjectType { get; }
        public Type DestinationObjectType { get; }
        public ICollection<FieldMap> Mappings { get; private set; }

        public ObjectMap(FieldMap map) 
            : this(map.Source.DeclaringType.Type, map.Destination.DeclaringType.Type, map.ProfileType)
        {
        }

        public ObjectMap(Type sourceObjectType, Type destinationObjectType) 
            : this(sourceObjectType, destinationObjectType, null)
        {
        }

        public ObjectMap(Type sourceObjectType, Type destinationObjectType, Type profileType)
        {
            SourceObjectType = sourceObjectType;
            DestinationObjectType = destinationObjectType;
            ProfileType = profileType;
            Mappings = new List<FieldMap>();
        }

        public void Add(FieldMap map)
        {
            if(!Mappings.Contains(map))
                Mappings.Add(map);
        }

        public override string ToString()
        {
            return $"{ProfileType?.Name ?? "none"}: {SourceObjectType.Name} => {DestinationObjectType.Name}";
        }
    }

    public class FieldMap : IEquatable<FieldMap>
    {
        public Type ProfileType { get; }
        public Field Source { get; }
        public Field Destination { get; }
        public bool IsRegistered { get; set; }

        public FieldMap(Field source, Field destination, bool isRegistered)
            :this(null, source, destination, isRegistered)
        {
        }

        public FieldMap(Type profileType, Field source, Field destination, bool isRegistered)
        {
            ProfileType = profileType;
            Source = source;
            Destination = destination;
            IsRegistered = isRegistered;
        }

        public override int GetHashCode()
        {
            var hashCode = 23;
            hashCode = hashCode * 31 + Source.GetHashCode();
            hashCode = hashCode * 31 + Destination.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(FieldMap))
                return false;
            var other = (FieldMap)obj;
            return Equals(other);
        }

        public bool Equals(FieldMap other)
        {
            if (other == null)
                return false;

            return Source == other.Source && Destination == other.Destination;
        }

        public override string ToString()
        {
            return $"{ProfileType?.Name ?? "none"}: {Source} => {Destination}{(IsRegistered ? "" : " (Ambient)")}";
        }
    }

    public class Field : IEquatable<Field>
    {
        public string Name { get; }
        public ExtendedType Type { get; }
        public ExtendedType DeclaringType { get; }
        public bool IsRegistered { get; }

        public Field(string name, Type type, Type declaringType, bool isRegistered = false)
        {
            Name = name;
            Type = type.GetExtendedType();
            DeclaringType = declaringType.GetExtendedType();
            IsRegistered = isRegistered;
        }

        public Field(string name, ExtendedType type, ExtendedType declaringType, bool isRegistered = false)
        {
            Name = name;
            Type = type;
            DeclaringType = declaringType;
            IsRegistered = isRegistered;
        }

        public override int GetHashCode()
        {
            var hashCode = 23;
            hashCode = hashCode * 31 + Name.GetHashCode();
            hashCode = hashCode * 31 + Type.Type.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Field))
                return false;
            var other = (Field)obj;
            return Equals(other);
        }

        public bool Equals(Field other)
        {
            if (other == null)
                return false;

            return Name == other.Name && Type == other.Type;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
