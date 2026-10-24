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

// RNF02: Cache HTTP em memória para garantir listagens públicas com tempo < 200ms
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromSeconds(60)));
    options.AddPolicy("CatalogoPublico", policy =>
        policy.Expire(TimeSpan.FromSeconds(120))
              .SetVaryByQuery("termo", "categoria", "regiao", "bairro", "tags", "abertoAgora", "notaMinima", "apenasGratuitos", "tipo"));
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
                title: "Regra de negócio violada",
                detail: domainEx.Message,
                statusCode: StatusCodes.Status400BadRequest).ExecuteAsync(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await Results.Problem(
            title: "Erro interno no servidor",
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

// RNF01: Endpoints de leitura 100% públicos (Zero Friction)
api.MapGet("/categorias", (string? tipo, ICatalogoService catalogoService) =>
{
    var categorias = catalogoService.ListarCategorias(tipo);
    return Results.Ok(categorias);
})
.CacheOutput("CatalogoPublico")
.WithName("ListarCategorias");

api.MapGet("/tags", (ICatalogoService catalogoService) =>
{
    var tags = catalogoService.ListarTags();
    return Results.Ok(tags);
})
.CacheOutput("CatalogoPublico")
.WithName("ListarTags");

api.MapGet("/locais", async (
    string? termo,
    string? categoria,
    UrbanRegion? regiao,
    string? bairro,
    string? tags,
    bool? abertoAgora,
    double? notaMinima,
    ICatalogoService catalogoService,
    CancellationToken cancellationToken) =>
{
    var listaTags = string.IsNullOrWhiteSpace(tags)
        ? null
        : tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    var criterios = new PlaceSearchCriteria(
        TermoBusca: termo,
        Categoria: CatalogoService.TentarConverterCategoria(categoria),
        Regiao: regiao,
        Bairro: bairro,
        EtiquetasClima: listaTags,
        ApenasAbertoAgora: abertoAgora ?? false,
        NotaMinima: notaMinima);

    var locais = await catalogoService.ListarLugaresAsync(criterios, cancellationToken);
    return Results.Ok(locais);
})
.CacheOutput("CatalogoPublico")
.WithName("ListarLocais");

api.MapGet("/locais/{idOuSlug}", async (
    string idOuSlug,
    ICatalogoService catalogoService,
    CancellationToken cancellationToken) =>
{
    var local = await catalogoService.ObterLugarPorIdOuSlugAsync(idOuSlug, cancellationToken: cancellationToken);
    return local is null ? Results.NotFound() : Results.Ok(local);
})
.CacheOutput("CatalogoPublico")
.WithName("ObterLocalPorIdOuSlug");

api.MapGet("/eventos", async (
    string? termo,
    UrbanRegion? regiao,
    string? tags,
    bool? apenasGratuitos,
    ICatalogoService catalogoService,
    CancellationToken cancellationToken) =>
{
    var listaTags = string.IsNullOrWhiteSpace(tags)
        ? null
        : tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    var criterios = new EventSearchCriteria(
        TermoBusca: termo,
        Regiao: regiao,
        EtiquetasClima: listaTags,
        ApenasGratuitos: apenasGratuitos ?? false);

    var eventos = await catalogoService.ListarEventosAtivosAsync(criterios, cancellationToken);
    return Results.Ok(eventos);
})
.CacheOutput("CatalogoPublico")
.WithName("ListarEventos");

api.MapGet("/eventos/{idOuSlug}", async (
    string idOuSlug,
    ICatalogoService catalogoService,
    CancellationToken cancellationToken) =>
{
    var evento = await catalogoService.ObterEventoPorIdOuSlugAsync(idOuSlug, cancellationToken);
    return evento is null ? Results.NotFound() : Results.Ok(evento);
})
.CacheOutput("CatalogoPublico")
.WithName("ObterEventoPorIdOuSlug");

// RF05 + RNF03: Recomendações inteligentes do Yupy AI com Fallback resiliente
api.MapPost("/recomendacoes", async (
    RecomendacaoRequestDto request,
    IRecomendacaoService recomendacaoService,
    CancellationToken cancellationToken) =>
{
    var resposta = await recomendacaoService.ObterRecomendacoesAsync(request, cancellationToken: cancellationToken);
    return Results.Ok(resposta);
})
.WithName("ObterRecomendacoesYupy");

app.Run();

public partial class Program;
