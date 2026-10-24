using YupAchei.Application.Interfaces;
using YupAchei.Application.Services;
using YupAchei.Domain.Repositories;
using YupAchei.Infrastructure.Repositories;
using YupAchei.Infrastructure.Services;

namespace YupAchei.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICatalogoService, CatalogoService>();
        services.AddScoped<IRecomendacaoService, RecomendacaoService>();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<ILugarRepository, InMemoryLugarRepository>();
        services.AddSingleton<IEventoRepository, InMemoryEventoRepository>();
        services.AddSingleton<InMemoryUserRepository>();
        services.AddSingleton<IUserRepository>(sp => sp.GetRequiredService<InMemoryUserRepository>());
        services.AddSingleton<IUserReviewRepository>(sp => sp.GetRequiredService<InMemoryUserRepository>());
        services.AddScoped<IRecomendacaoAgente, RecomendacaoAgenteLocalService>();
        return services;
    }
}
