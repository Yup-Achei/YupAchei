using Microsoft.AspNetCore.Diagnostics;
using YupAchei.API.Extensions;
using YupAchei.Application.DTOs;
using YupAchei.Application.Interfaces;
using YupAchei.Application.Services;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;
using YupAchei.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

// RNF02: In-memory HTTP Output Cache to keep public listings < 200ms
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromSeconds(60)));
    options.AddPolicy("PublicCatalog", policy =>
        policy.Expire(TimeSpan.FromSeconds(120))
              .SetVaryByQuery("search", "category", "region", "neighborhood", "tags", "openNow", "minRating", "freeOnly", "type"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000",
                "http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

var app = builder.Build();

app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is DomainException domainEx)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await Results.Problem(
                title: "Business rule violation",
                detail: domainEx.Message,
                statusCode: StatusCodes.Status400BadRequest).ExecuteAsync(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await Results.Problem(
            title: "Internal server error",
            statusCode: StatusCodes.Status500InternalServerError).ExecuteAsync(context);
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("FrontendPolicy");
app.UseOutputCache();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "YupAchei.API" }))
   .WithName("HealthCheck");

var api = app.MapGroup("/api");

// RNF01: 100% public read endpoints (Zero Friction)
api.MapGet("/categories", (string? type, ICatalogService catalogService) =>
{
    var categories = catalogService.ListCategories(type);
    return Results.Ok(categories);
})
.CacheOutput("PublicCatalog")
.WithName("GetCategories");

api.MapGet("/tags", (ICatalogService catalogService) =>
{
    var tags = catalogService.ListTags();
    return Results.Ok(tags);
})
.CacheOutput("PublicCatalog")
.WithName("GetTags");

api.MapGet("/places", async (
    string? search,
    string? category,
    UrbanRegion? region,
    string? neighborhood,
    string? tags,
    bool? openNow,
    double? minRating,
    ICatalogService catalogService,
    CancellationToken cancellationToken) =>
{
    var tagList = string.IsNullOrWhiteSpace(tags)
        ? null
        : tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    var criteria = new PlaceSearchCriteria(
        SearchTerm: search,
        Category: CatalogService.TryParseCategory(category),
        Region: region,
        Neighborhood: neighborhood,
        VibeTags: tagList,
        OnlyOpenNow: openNow ?? false,
        MinRating: minRating);

    var places = await catalogService.ListPlacesAsync(criteria, cancellationToken);
    return Results.Ok(places);
})
.CacheOutput("PublicCatalog")
.WithName("GetPlaces");

api.MapGet("/places/{idOrSlug}", async (
    string idOrSlug,
    ICatalogService catalogService,
    CancellationToken cancellationToken) =>
{
    var place = await catalogService.GetPlaceByIdOrSlugAsync(idOrSlug, cancellationToken: cancellationToken);
    return place is null ? Results.NotFound() : Results.Ok(place);
})
.CacheOutput("PublicCatalog")
.WithName("GetPlaceByIdOrSlug");

api.MapGet("/events", async (
    string? search,
    UrbanRegion? region,
    string? tags,
    bool? freeOnly,
    ICatalogService catalogService,
    CancellationToken cancellationToken) =>
{
    var tagList = string.IsNullOrWhiteSpace(tags)
        ? null
        : tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    var criteria = new EventSearchCriteria(
        SearchTerm: search,
        Region: region,
        VibeTags: tagList,
        OnlyFree: freeOnly ?? false);

    var events = await catalogService.ListActiveEventsAsync(criteria, cancellationToken);
    return Results.Ok(events);
})
.CacheOutput("PublicCatalog")
.WithName("GetEvents");

api.MapGet("/events/{idOrSlug}", async (
    string idOrSlug,
    ICatalogService catalogService,
    CancellationToken cancellationToken) =>
{
    var @event = await catalogService.GetEventByIdOrSlugAsync(idOrSlug, cancellationToken);
    return @event is null ? Results.NotFound() : Results.Ok(@event);
})
.CacheOutput("PublicCatalog")
.WithName("GetEventByIdOrSlug");

// RF05 + RNF03: Yupy AI smart recommendations with resilient fallback
api.MapPost("/recommendations", async (
    RecommendationRequestDto request,
    IRecommendationService recommendationService,
    CancellationToken cancellationToken) =>
{
    var response = await recommendationService.GetRecommendationsAsync(request, cancellationToken: cancellationToken);
    return Results.Ok(response);
})
.WithName("GetYupyRecommendations");

app.Run();

public partial class Program;
