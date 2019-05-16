using System;
using System.Collections.Generic;

namespace AnyMapper.Tests.TestObjects
{
    public class DestObject
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public ICollection<SimpleObject> Items { get; set; }

        public override string ToString()
        {
            return $"Id: {Id} Name: {Name} DateCreated: {DateCreated} Description: {Description} IsEnabled: {IsEnabled}";
        }
    }
}
