using System;
using System.Collections.Generic;
using System.Text;

namespace AnyMapper.Tests.TestObjects
{
    public class DestObject
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }

        public string Description { get; set; }
        public bool IsEnabled { get; set; }
    }
}
