namespace YupAchei.Application.DTOs;

public sealed record CategoryDto(
    string Id,
    string Name,
    IReadOnlyList<string> Types,
    string Icon,
    string Color,
    int Order);

public sealed record TagDto(
    string Id,
    string Name,
    string Icon);

public sealed record AddressDto(
    string? Street,
    string Number,
    string? Neighborhood,
    string City,
    string State,
    string? ZipCode);

public sealed record CoordinatesDto(
    double Lat,
    double Lng);

public sealed record OpeningHoursDto(
    string Day,
    string Open,
    string Close);

public sealed record ContactDto(
    string? Instagram,
    string? WhatsApp,
    string? Phone,
    string? Email,
    string? Website);

public sealed record PlaceImagesDto(
    string Main,
    IReadOnlyList<string> Gallery);

public sealed record RatingSummaryDto(
    double Average,
    int Total);

public sealed record VerificationDto(
    bool Address,
    bool Schedule,
    bool Coordinates,
    IReadOnlyList<string> Sources,
    string? Notes = null);

public sealed record PartnerReviewDto(
    Guid Id,
    Guid PartnerId,
    string? PartnerName,
    string? AvatarUrl,
    string? SocialNetwork,
    string? SocialProfileUrl,
    string BadgeOrScore,
    string ShortComment,
    bool IsFeatured);

public sealed record PriceRangeDto(
    bool IsFree,
    decimal MinAmount,
    decimal MaxAmount,
    string Currency);

public sealed record PlaceDto(
    string Id,
    string Slug,
    string Name,
    string CategoryId,
    string Kind,
    string Description,
    AddressDto Address,
    CoordinatesDto Coordinates,
    PriceRangeDto PriceRange,
    bool IsOpen24Hours,
    bool IsOpenNow,
    IReadOnlyList<OpeningHoursDto> OpeningHours,
    string? Notes,
    IReadOnlyList<string> Tags,
    ContactDto Contact,
    PlaceImagesDto Images,
    RatingSummaryDto Rating,
    IReadOnlyList<PartnerReviewDto> PartnerReviews,
    string RecommendedBy,
    bool IsSponsored,
    string Status,
    VerificationDto Verification,
    string UpdatedAt);

public sealed record FreeLocationDto(
    string Name,
    string? AddressText,
    string? Neighborhood,
    CoordinatesDto Coordinates);

public sealed record OrganizerDto(
    string Name,
    string? PublicContact,
    bool IsVerified);

public sealed record EventDto(
    string Id,
    string Slug,
    bool IsSimulated,
    string Title,
    string CategoryId,
    string Description,
    string StartDate,
    string EndDate,
    string? PlaceId,
    FreeLocationDto? FreeLocation,
    PriceRangeDto PriceRange,
    string? TicketUrl,
    string Image,
    OrganizerDto? Organizer,
    IReadOnlyList<string> Tags,
    bool IsFeatured,
    bool IsSponsored,
    string Status);

public sealed record RecommendationRequestDto(string Intent);

public sealed record RecommendationResponseDto(
    string Intent,
    IReadOnlyList<PlaceDto> Suggestions,
    bool UsedFallback,
    string Message);
