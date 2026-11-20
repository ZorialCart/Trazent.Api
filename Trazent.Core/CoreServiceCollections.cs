using Microsoft.Extensions.DependencyInjection;
using Trazent.Api.Core.Implementations.Repositories;
using Trazent.Api.Core.Implementations.Services;
using Trazent.Api.Core.Infraestructure.Repositories;
using Trazent.Api.DataAccess;
using Trazent.Core.Implementations.Repositories;
using Trazent.Core.Implementations.Services;
using Trazent.Core.Infraestructure.Repositories;
using Trazent.Core.Infraestructure.Services;

namespace Trazent.Core;

public static class CoreServiceCollections
{
    public static void RegisterCoreServices(this IServiceCollection services)
    {
        RegistrarRepositories(services);
        RegistrarService(services);
    }

    private static void RegistrarRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAppDbContext, AppDbContext>();
        services.AddScoped<ICoreUnitRepositories, CoreUnitRepositories>();
        services.AddScoped<ICompaniesRepository, CompaniesRepository>();
        services.AddScoped<ISitesRepository, SitesRepository>();
    }

    private static void RegistrarService(this IServiceCollection services)
    {
        services.AddScoped<ICompaniesServices, CompaniesServices>();
        services.AddScoped<ISitesServices, SitesServices>();
    }


    //public static void RegisterHttpClientServices(this IServiceCollection services)
    //{
    //    services.AddHttpClient<IAppSyncEventsService, AppSyncEventsService>();
    //}
}
