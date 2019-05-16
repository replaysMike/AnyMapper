using System;
using System.Collections.Generic;
using System.Text;

namespace AnyMapper.Tests.TestObjects
{
    public class SourceObject
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public ICollection<SimpleObject> Items { get; set; }

        public override string ToString()
        {
            return $"Id: {Id} Name: {Name} DateCreated: {DateCreated}";
        }
    }
}
