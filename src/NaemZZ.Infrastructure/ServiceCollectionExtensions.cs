using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NaemZZ.Core.Interfaces;
using NaemZZ.Infrastructure.Data;
using NaemZZ.Infrastructure.Services;

namespace NaemZZ.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the SQLite-backed EF Core persistence layer and application services.
    /// </summary>
    /// <param name="databaseFilePath">Full path to the SQLite database file.</param>
    public static IServiceCollection AddNaemZZInfrastructure(this IServiceCollection services, string databaseFilePath)
    {
        services.AddDbContextFactory<NaemZZDbContext>(options =>
            options.UseSqlite($"Data Source={databaseFilePath}"));

        // ILandlordService owns a tracked DbContext for the lifetime of one page visit:
        // it must be Scoped, and the Components layer resolves it from a fresh IServiceScope
        // per page (see LandlordsPage), not from the application's root scope.
        services.AddScoped<ILandlordService, LandlordService>();
        services.AddSingleton<IFirmManagementService, FirmManagementService>();

        // Lookup tables rarely change; a single cached singleton avoids re-querying them.
        services.AddSingleton<ILookupService, LookupService>();

        return services;
    }
}
