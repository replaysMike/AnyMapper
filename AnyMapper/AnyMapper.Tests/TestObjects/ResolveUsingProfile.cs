namespace AnyMapper.Tests.TestObjects
{
    public class ResolveUsingProfile : Profile
    {
        public ResolveUsingProfile()
        {
            CreateMap<SourceObject, UniqueObject>()
                .ForMember(x => x.UserId, x => Resolve(x.Id))
                //.ForMember(x => x.FullName, x => Resolve(Something(x)))
                .ForMember(x => x.FullName, (x, context) => { return x.Name.Replace("S", "s"); })
                //.ForMember(x => x.FullName, x => x.Name)
            ;
        }

        public string Something(SourceObject x)
        {
            return x.Name.Replace("M", "m");
        }
    }
}
