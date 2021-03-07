using System;
using System.Collections.Generic;

namespace AnyMapper.Tests.ComplexObjects
{
    public static class ComplexObjectFactory
    {
        public static ns1.Inventory CreateSspInventory()
        {
            var rand = new Random();
            
            return new ns1.Inventory()
            {
                AdvertiserOrganizationId = rand.Next(1, 10),
                AssignedBuySpecificationId = rand.Next(1, 10),
                BrandOrganizationId = rand.Next(1, 10),
                BuyerOrganizationId = rand.Next(1, 10),
                BuylineTypeId = rand.Next(1, 3),
                ChangesetItemHash = Guid.Empty,
                ClonedFromId = rand.Next(1, 10),
                Cost = rand.Next(2, 100),
                CreatedByName = "Test",
                CustomDayPartId = rand.Next(1, 5),
                DateCreatedUtc = new DateTime(2021, 1, 2),
                DateModifiedUtc = new DateTime(2021, 1, 2),
                DateDisabledUtc = null,
                DayPartId = rand.Next(1, 5),
                DaysOfWeek = new ns1.DaysOfWeek(),
                DealStatuses = new List<int> { 1, 2, 3 },
                Description = "Description",
                StartTime = "4:00",
                EndTime = "5:00",
                FileAssignmentVersion = 2,
                HasUpdate = false,
                Id = rand.Next(1, 1000),
                ImportVersion = 1,
                InventoryPackageId = rand.Next(1, 10),
                InventoryPackageName = "Test",
                InventoryPoolCollectionId = rand.Next(1, 10),
                InventorySideTypeId = 1,
                InventorySourceTypeId = 1,
                InventoryStatusTypeId = 1,
                IsDisabled = false,
                IsPerfectMatch = true,
                IsSubmitted = true,
                IsVirtual = false,
                LengthId = 5,
                MarketId = rand.Next(1, 10),
                MarketZone = "E",
                MasterInventoryId = rand.Next(1, 5),
                MediaTypeId = 1,
                OriginalCost = rand.Next(2, 100),
                PremiumIsRestricted = false,
                ModifiedByName = "None",
                TotalUnits = 10,
                Quarter = "Q1",
                OriginalProgramName = "Test",
                Year = 2021,
                ThirdPartyDataIndex = 1m,
                TransferTypeId = 1,
                ProgramIds = new List<int> { 1, 2, 3 },
                PremiumPercentage = rand.Next(1, 5),
                StatsProviderId = 1,
                UseAllocatedWeeksIntersectionForEstimates = true,
                Allocations = new ns1.InventoryAllocations { Actual = new List<int> { 1, 2, 3, 4, 5 }, Default = new List<int> { 1, 2, 3, 4, 5 } },
                AssignedBuySpecification = new ns1.BuySpecificationMinimal(),
                Estimate = new List<ns1.AudienceEstimate> { new ns1.AudienceEstimate { AutoUpdate = false, DemographicId = 1, HutValue = 2m, PutValue = 3m } },
                EstimateAlgorithmTypeId = 1,
                StationOrganizationId = rand.Next(1, 50),
                SupplierSpecifiedDemographicId = rand.Next(1, 10),
                InventoryGroups = new List<ns1.InventoryGroup> { },
                SyncedEstimate = new List<ns1.AudienceEstimate> { },
                SupplierSpecifiedImpressionsEstimatePerK = 3m,
                SystemEstimate = new List<ns1.AudienceEstimate> { }
            };
        }
    }
}
