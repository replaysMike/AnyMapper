#if FEATURE_EF
using System.Data.Entity;

namespace AnyMapper.Tests.EF6
{
    public partial class EFDbContext : DbContext
    {
        public virtual DbSet<DbObject> DbObjects { get; set; }
        public virtual DbSet<ChildDbObject> ChildDbObjects { get; set; }

        public EFDbContext() : base()
        {
        }
    }
}
#endif
