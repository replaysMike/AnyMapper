namespace AnyMapper.Tests.TestObjects
{
    public class SimpleObject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public SimpleObject(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var typedObj = (SimpleObject)obj;
            return typedObj.Id.Equals(Id) && typedObj.Name.Equals(Name);
        }
    }
}
