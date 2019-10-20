using System;
using System.Collections.Generic;
using System.Text;

namespace AnyMapper.Tests.TestObjects
{
    public class SourceObject
    {
        private readonly string _readOnlyField = "";

        public string Name { get; set; }
        public int Id { get; set; }
        public int ReadOnlyId { get; }
        public DateTime DateCreated { get; set; }
        public ICollection<SimpleObject> Items { get; set; }

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
    }
}
