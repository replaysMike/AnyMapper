namespace AnyMapper.Tests.TestObjects
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<SourceObject, DestObject>()
                .ForMember(x => x.Id, x => x.Id)
                .ForMember(x => x.Name, x => x.Name)
                .ForMember(x => x.DateCreated, x => x.DateCreated)
            ;
        }
    }
}
