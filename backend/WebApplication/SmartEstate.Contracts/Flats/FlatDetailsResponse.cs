using Presentation.Contracts.Building;
using Presentation.Contracts.Developer;
using Presentation.Contracts.InfrastructureInfo;
using Presentation.Contracts.Metro;

namespace Presentation.Contracts.Flats;

public record FlatDetailsResponse(
    int FlatId,
    string[] Images,
    decimal Square,
    int Roominess,
    int Floor,
    string CianLink,
    string FinishType,
    decimal Price,
    string PriceChart,
    int BuildingId,
    int DeveloperId,
    int InfrastructureInfoId,
    
    BuildingInfoDto BuildingInfo,
    DeveloperInfoDto DeveloperInfo,
    
    NearestMetroInfo? NearestMetroInfo,
    NearestSchoolInfo? NearestSchoolInfo,
    NearestKindergartenInfo? NearestKindergartenInfo,
    NearestShopInfo? NearestShopInfo,
    NearestPharmacyInfo? NearestPharmacyInfo);
