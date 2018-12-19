using System;
using System.Collections.Generic;
using System.Text;

namespace AnyMapper
{
    public class MappingContext
    {
        public static MappingContext None { get { return new MappingContext(); } }

        public Dictionary<string, object> Items { get; }

        public MappingContext()
        {
            Items = new Dictionary<string, object>();
        }

        public void Add(string name, object item)
        {
            Items.Add(name, item);
        }

    }
}
