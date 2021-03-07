using System;

namespace AnyMapper.Tests.TestObjects
{
    public class Person : IEquatable<Person>
    {
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public int Age => DateTime.Now.Subtract(DOB).Days / 365;

        public bool Equals(Person other)
        {
            return Name.Equals(other.Name)
                && DOB.Equals(other.DOB)
                && Age.Equals(other.Age);
        }
    }
}
