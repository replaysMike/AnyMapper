using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnyMapper.Tests.TestObjects
{
    public class SourceObject : IEquatable<SourceObject>, IEquatable<DestObject>
    {
        private readonly string _readOnlyField = "";

        public string Name { get; set; }
        public int Id { get; set; }
        public int ReadOnlyId { get; }
        public DateTime DateCreated { get; set; }
        public ICollection<SimpleObject> Items { get; set; }
        public int NullableInt { get; set; }

        public SourceObject() { }
        public SourceObject(int readOnlyPropertyId)
        {
            ReadOnlyId = readOnlyPropertyId;
        }

        public SourceObject(string readOnlyField)
        {
            _readOnlyField = readOnlyField;
        }
        public bool ValidateReadOnlyField(string val)
        {
            return val == _readOnlyField;
        }

        public override string ToString()
        {
            return $"Id: {Id} Name: {Name} DateCreated: {DateCreated}";
        }

        public bool Equals(SourceObject other)
        {
            return Name.Equals(other.Name)
                && Id.Equals(other.Id)
                && ReadOnlyId.Equals(other.ReadOnlyId)
                && DateCreated.Equals(other.DateCreated)
                && (Items == null || Items.SequenceEqual(other.Items))
                && NullableInt.Equals(other.NullableInt);
        }

        public bool Equals(DestObject other)
        {
            return Name.Equals(other.Name)
                && Id.Equals(other.Id)
                && ReadOnlyId.Equals(other.ReadOnlyId)
                && DateCreated.Equals(other.DateCreated)
                && (Items == null || Items.SequenceEqual(other.Items))
                && NullableInt.Equals(other.NullableInt);
        }
    }
}
