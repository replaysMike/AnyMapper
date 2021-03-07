using System;
using System.Collections.Generic;
using System.Linq;

namespace AnyMapper.Tests.ComplexObjects.ns2
{
    public class Inventory : InventoryBase, IVirtual
    {
        public int ExtraFieldThatDoesntExist { get; set; }
        public long? MasterInventoryId { get; set; }

        public int? BuyerOrganizationId { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsSubmitted { get; set; }

        public int? AssignedBuySpecificationId { get; set; }

        public BuySpecificationMinimal AssignedBuySpecification { get; set; }

        public List<int> DealStatuses { get; set; }

        public bool IsDisabled { get; set; }
    }

    public interface IVirtual
    {
        bool IsVirtual { get; set; }
    }

    public abstract class InventoryBase : AuditableBase, IInventory
    {
        public virtual long Id { get; set; }

        public virtual List<int> ProgramIds { get; set; } = new List<int>();

        public virtual int DayPartId { get; set; }

        public virtual int? CustomDayPartId { get; set; }

        public virtual int? AdvertiserOrganizationId { get; set; }

        public virtual int? BrandOrganizationId { get; set; }

        public virtual int? InventoryPoolCollectionId { get; set; }

        public virtual string StartTime { get; set; }

        public virtual string EndTime { get; set; }

        public virtual DaysOfWeek DaysOfWeek { get; set; } = new DaysOfWeek();

        public virtual int? LengthId { get; set; }

        public virtual int MarketId { get; set; }

        public virtual int MediaTypeId { get; set; }

        public virtual string Description { get; set; }

        public virtual decimal? Cost { get; set; }

        public virtual decimal? OriginalCost { get; set; }

        public virtual int? Year { get; set; }

        public virtual string Quarter { get; set; }

        public virtual int BuylineTypeId { get; set; }

        public virtual int TotalUnits { get; set; }

        public virtual bool IsPerfectMatch { get; set; }

        public virtual int InventorySourceTypeId { get; set; }
        public virtual int InventorySideTypeId { get; set; }

        public virtual int InventoryStatusTypeId { get; set; }

        public virtual long? InventoryPackageId { get; set; }

        public virtual string InventoryPackageName { get; set; }

        public virtual decimal? PremiumPercentage { get; set; }

        public virtual bool PremiumIsRestricted { get; set; }
        public virtual int? StationOrganizationId { get; set; }

        public virtual string MarketZone { get; set; }

        public virtual int StatsProviderId { get; set; }

        public virtual long? ClonedFromId { get; set; }

        public virtual Guid ChangesetItemHash { get; set; }

        public virtual DateTime? DateDisabledUtc { get; set; }

        public virtual int? TransferTypeId { get; set; }
        public virtual bool HasUpdate { get; set; }

        public virtual int EstimateAlgorithmTypeId { get; set; }

        public virtual bool UseAllocatedWeeksIntersectionForEstimates { get; set; }


        public virtual string OriginalProgramName { get; set; }


        public virtual int? ImportVersion { get; set; }

        public virtual int? FileAssignmentVersion { get; set; }

        public virtual decimal ThirdPartyDataIndex { get; set; }

        public virtual decimal? SupplierSpecifiedImpressionsEstimatePerK { get; set; }


        public virtual int? SupplierSpecifiedDemographicId { get; set; }


        public virtual InventoryAllocations Allocations { get; set; } = new InventoryAllocations();


        public virtual List<AudienceEstimate> Estimate { get; set; } = new List<AudienceEstimate>();

        public virtual List<AudienceEstimate> SyncedEstimate { get; set; } = new List<AudienceEstimate>();


        public virtual List<AudienceEstimate> SystemEstimate { get; set; } = new List<AudienceEstimate>();

        public virtual List<InventoryGroup> InventoryGroups { get; set; } = new List<InventoryGroup>();
    }

    public abstract class AuditableBase : IAuditable
    {
        public virtual string CreatedByName { get; set; }

        public virtual string ModifiedByName { get; set; }

        public virtual DateTime DateCreatedUtc { get; set; }

        public virtual DateTime DateModifiedUtc { get; set; }
    }

    public interface IAuditable : IAuditableDateTime, IAuditableName { }

    public interface IAuditableDateTime
    {
        DateTime DateCreatedUtc { get; set; }

        DateTime DateModifiedUtc { get; set; }
    }

    public interface IAuditableName
    {
        string CreatedByName { get; set; }

        string ModifiedByName { get; set; }
    }

    public class DaysOfWeek : IEquatable<DaysOfWeek>
    {
        public bool? Mon { get; set; }

        public bool? Tue { get; set; }

        public bool? Wed { get; set; }

        public bool? Thu { get; set; }
        public bool? Fri { get; set; }
        public bool? Sat { get; set; }
        public bool? Sun { get; set; }

        public DaysOfWeek()
        {
        }

        public DaysOfWeek(int fromFlags)
        {
            var days = (InternalDaysOfWeek)fromFlags;
            Mon = days.HasFlag(InternalDaysOfWeek.Monday);
            Tue = days.HasFlag(InternalDaysOfWeek.Tuesday);
            Wed = days.HasFlag(InternalDaysOfWeek.Wednesday);
            Thu = days.HasFlag(InternalDaysOfWeek.Thursday);
            Fri = days.HasFlag(InternalDaysOfWeek.Friday);
            Sat = days.HasFlag(InternalDaysOfWeek.Saturday);
            Sun = days.HasFlag(InternalDaysOfWeek.Sunday);
        }

        public static DaysOfWeek FromFlags(int flags) => new DaysOfWeek(flags);

        public int ToFlags()
        {
            var flags = Mon ?? false ? (int)InternalDaysOfWeek.Monday : 0;
            flags |= Tue ?? false ? (int)InternalDaysOfWeek.Tuesday : 0;
            flags |= Wed ?? false ? (int)InternalDaysOfWeek.Wednesday : 0;
            flags |= Thu ?? false ? (int)InternalDaysOfWeek.Thursday : 0;
            flags |= Fri ?? false ? (int)InternalDaysOfWeek.Friday : 0;
            flags |= Sat ?? false ? (int)InternalDaysOfWeek.Saturday : 0;
            flags |= Sun ?? false ? (int)InternalDaysOfWeek.Sunday : 0;
            return flags;
        }

        public override bool Equals(object obj)
            => obj is DaysOfWeek other && Equals(other);

        public bool Equals(DaysOfWeek other)
            => Mon == other.Mon
                && Tue == other.Tue
                && Wed == other.Wed
                && Thu == other.Thu
                && Fri == other.Fri
                && Sat == other.Sat
                && Sun == other.Sun;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Mon.GetHashCode();
                hashCode = (hashCode * 397) ^ Tue.GetHashCode();
                hashCode = (hashCode * 397) ^ Wed.GetHashCode();
                hashCode = (hashCode * 397) ^ Thu.GetHashCode();
                hashCode = (hashCode * 397) ^ Fri.GetHashCode();
                hashCode = (hashCode * 397) ^ Sat.GetHashCode();
                hashCode = (hashCode * 397) ^ Sun.GetHashCode();
                return hashCode;
            }
        }
    }

    public class InventoryAllocations
    {
        public IList<int> Actual { get; set; }

        public IList<int> Default { get; set; }

        public InventoryAllocations()
        {
            Actual = new List<int>();
            Default = new List<int>();
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + Actual?.GetHashCode() ?? 0;
            hash = hash * 23 + Default?.GetHashCode() ?? 0;
            return hash;
        }

        public override bool Equals(object o)
        {
            if (!(o is InventoryAllocations other))
            {
                return false;
            }

            return other.Actual.SequenceEqual(Actual);
        }
    }

    [Flags]
    public enum InternalDaysOfWeek
    {
        Monday = 0x0001,
        Tuesday = 0x0002,
        Wednesday = 0x0004,
        Thursday = 0x0008,
        Friday = 0x0010,
        Saturday = 0x0020,
        Sunday = 0x0040,
    }

    public class AudienceEstimate
    {
        public long Id { get; set; }

        public int EstimateTypeId { get; set; }

        public int DemographicId { get; set; }

        public bool AutoUpdate { get; set; }

        public decimal Value { get; set; }

        public decimal? RatingsValue { get; set; }

        public decimal HutValue { get; set; }

        public decimal PutValue { get; set; }

        public bool IsUserOverride { get; set; } = false;

        public decimal Universe { get; set; }

        public DateTime? DateModifiedUtc { get; set; }
    }

    public class InventoryGroup
    {
        public Guid ChangesetItemHash { get; set; }
        public int InventoryGroupTypeId { get; set; }
    }

    public interface IInventory : IId<long>
    {
        List<int> ProgramIds { get; set; }
        int DayPartId { get; set; }
        string StartTime { get; set; }

        string EndTime { get; set; }

        DaysOfWeek DaysOfWeek { get; set; }
        int? LengthId { get; set; }

        int MarketId { get; set; }

        int MediaTypeId { get; set; }

        decimal? Cost { get; set; }
        int? Year { get; set; }

        string Quarter { get; set; }

        InventoryAllocations Allocations { get; set; }

        decimal? PremiumPercentage { get; set; }

        bool PremiumIsRestricted { get; set; }
        int? StationOrganizationId { get; set; }
    }

    public interface IId<T> where T : struct
    {
        T Id { get; set; }
    }

    public class BuySpecificationMinimal : AuditableBase, IId<int>
    {
        public int Id { get; set; }

        public int PortfolioId { get; set; }

        public string PortfolioName { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int AdvertiserOrganizationId { get; set; }
        public int BrandOrganizationId { get; set; }
        public int[] MediaTypeIds { get; set; }
        public int MarketTypeId { get; set; }

        public int DemographicValueTypeId { get; set; }

        public int GoalsMetricTypeId { get; set; }

        public IList<int> MarketsIds { get; set; }

        public IList<int> ExcludedProgramIds { get; set; }
        public IList<int> ExcludedProgramCategoryIds { get; set; }

        public IList<int> ExcludedStationOrganizationIds { get; set; }
        public int PrimaryDemographicId { get; set; }
        public IList<int> AlternateDemographicIds { get; set; }

        public Guid ChangesetItemHash { get; set; }

        public int NielsenStreamTypeId { get; set; }
    }
}
