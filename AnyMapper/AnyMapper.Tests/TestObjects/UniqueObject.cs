using System;

namespace AnyMapper.Tests.TestObjects
{
    public class UniqueObject
    {
        public string FullName { get; set; }
        public int UserId { get; set; }
        public DateTime LogTime { get; set; }

        public override string ToString()
        {
            return $"UserId: {UserId} FullName: {FullName} LogTime: {LogTime}";
        }
    }
}
