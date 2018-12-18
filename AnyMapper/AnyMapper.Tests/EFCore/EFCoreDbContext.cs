#if FEATURE_EFCORE
using Microsoft.EntityFrameworkCore;

namespace AnyMapper.Tests.EFCore
{
    public partial class EFCoreDbContext : DbContext
    {
        public virtual DbSet<DbObject> DbObjects { get; set; }
        public virtual DbSet<ChildDbObject> ChildDbObjects { get; set; }

        public EFCoreDbContext() : base()
        {
        }

        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options)
        : base(options)
        { }
    }
}
#endif
