using System;
using System.Collections.Generic;

namespace AnyMapper.Tests.TestObjects
{
    public class DestObject
    {
        private readonly string _readOnlyField = "";

        public string Name { get; set; }
        public int Id { get; set; }
        public int ReadOnlyId { get; }
        public DateTime DateCreated { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public ICollection<SimpleObject> Items { get; set; }
        public int? NullableInt { get; set; }

        public DestObject() { }
        public DestObject(int readOnlyPropertyId)
        {
            ReadOnlyId = readOnlyPropertyId;
        }

        public DestObject(string readOnlyField)
        {
            _readOnlyField = readOnlyField;
        }

        public bool ValidateReadOnlyField(string val)
        {
            return val == _readOnlyField;
        }

        public override string ToString()
        {
            return $"Id: {Id} Name: {Name} DateCreated: {DateCreated} Description: {Description} IsEnabled: {IsEnabled}";
        }
    }
}
