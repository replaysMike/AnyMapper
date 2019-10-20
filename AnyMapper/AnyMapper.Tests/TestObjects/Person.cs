using System;

namespace AnyMapper.Tests.TestObjects
{
    public class Person
    {
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public int Age => DateTime.Now.Subtract(DOB).Days / 365;
    }
}
