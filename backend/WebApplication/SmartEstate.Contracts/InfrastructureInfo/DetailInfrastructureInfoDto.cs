using Presentation.Contracts.Metro;

namespace Presentation.Contracts.InfrastructureInfo;

public record DetailInfrastructureInfoDto(
    NearestMetroInfo NearestMetroInfo,
    NearestKindergartenInfo NearestKindergartenInfo,
    NearestPharmacyInfo NearestPharmacyInfo,
    NearestSchoolInfo NearestSchoolInfo,
    NearestShopInfo NearestShopInfo);