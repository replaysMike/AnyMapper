namespace AnyMapper
{
    /// <summary>
    /// Resolves an instance of Mapper
    /// </summary>
    public class MapperResolver
    {
        public Mapper Resolve()
        {
            // todo: this is temporary, an actual context will be introduced
            return Mapper.Instance;
        }
    }
}
