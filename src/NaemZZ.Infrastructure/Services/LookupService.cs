using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Interfaces;
using NaemZZ.Infrastructure.Data;

namespace NaemZZ.Infrastructure.Services;

/// <summary>
/// Caches the small, rarely-changing lookup tables (Company, PropertyCategory, Region, Municipality, Settlement)
/// in memory to avoid repeated round-trips for cascading dropdowns. Safe to register as a
/// Singleton: each read uses a short-lived DbContext from the factory.
/// </summary>
public class LookupService : ILookupService
{
    private readonly IDbContextFactory<NaemZZDbContext> _factory;
    private readonly ConcurrentDictionary<string, object> _cache = new();

    public LookupService(IDbContextFactory<NaemZZDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<Company>> GetFirmsAsync(CancellationToken ct = default) =>
        await GetOrAddAsync("firms", async () =>
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.Company.AsNoTracking().OrderBy(f => f.Name).ToListAsync(ct);
        });

    public async Task<IReadOnlyList<PropertyCategory>> GetCategoriesAsync(CancellationToken ct = default) =>
        await GetOrAddAsync("categories", async () =>
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.PropertyCategory.AsNoTracking().OrderBy(c => c.NAME).ToListAsync(ct);
        });

    public async Task<IReadOnlyList<Region>> GetRegionsAsync(CancellationToken ct = default) =>
        await GetOrAddAsync("regions", async () =>
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.Region.AsNoTracking().OrderBy(o => o.NAME).ToListAsync(ct);
        });

    public async Task<IReadOnlyList<Municipality>> GetMunicipalitiesAsync(int regionId, CancellationToken ct = default) =>
        await GetOrAddAsync($"municipalities:{regionId}", async () =>
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.Municipality.AsNoTracking()
                .Where(o => o.OBL_ID == regionId)
                .OrderBy(o => o.NAME)
                .ToListAsync(ct);
        });

    public async Task<IReadOnlyList<Settlement>> GetSettlementsAsync(int municipalityId, CancellationToken ct = default) =>
        await GetOrAddAsync($"settlements:{municipalityId}", async () =>
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.Settlement.AsNoTracking()
                .Where(e => e.OBST_ID == municipalityId)
                .OrderBy(e => e.NAME)
                .ToListAsync(ct);
        });

    public void InvalidateCache() => _cache.Clear();

    private async Task<IReadOnlyList<T>> GetOrAddAsync<T>(string key, Func<Task<List<T>>> factory)
    {
        if (_cache.TryGetValue(key, out var cached))
        {
            return (IReadOnlyList<T>)cached;
        }

        var result = await factory();
        _cache[key] = result;
        return result;
    }
}
