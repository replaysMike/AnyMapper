namespace AnyMapper.Tests.TestObjects
{
    public class UniqueProfile : Profile
    {
        public UniqueProfile()
        {
            CreateMap<SourceObject, UniqueObject>()
                .ForMember(x => x.UserId, x => x.Id)
                .ForMember(x => x.FullName, x => x.Name)
            ;
        }
    }
}
