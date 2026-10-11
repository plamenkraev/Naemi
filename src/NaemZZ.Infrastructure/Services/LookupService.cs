using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using NaemZZ.Core.Entities;
using NaemZZ.Core.Interfaces;
using NaemZZ.Infrastructure.Data;

namespace NaemZZ.Infrastructure.Services;

/// <summary>
/// Caches the small, rarely-changing lookup tables (FIRM, CATEGORI, EK_OBL, EK_OBST, EKATTE)
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

    public async Task<IReadOnlyList<FIRM>> GetFirmsAsync(CancellationToken ct = default) =>
        await GetOrAddAsync("firms", async () =>
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.FIRM.AsNoTracking().OrderBy(f => f.firmNAME).ToListAsync(ct);
        });

    public async Task<IReadOnlyList<CATEGORI>> GetCategoriesAsync(CancellationToken ct = default) =>
        await GetOrAddAsync("categories", async () =>
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.CATEGORI.AsNoTracking().OrderBy(c => c.NAME).ToListAsync(ct);
        });

    public async Task<IReadOnlyList<EK_OBL>> GetRegionsAsync(CancellationToken ct = default) =>
        await GetOrAddAsync("regions", async () =>
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.EK_OBL.AsNoTracking().OrderBy(o => o.NAME).ToListAsync(ct);
        });

    public async Task<IReadOnlyList<EK_OBST>> GetMunicipalitiesAsync(int regionId, CancellationToken ct = default) =>
        await GetOrAddAsync($"municipalities:{regionId}", async () =>
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.EK_OBST.AsNoTracking()
                .Where(o => o.OBL_ID == regionId)
                .OrderBy(o => o.NAME)
                .ToListAsync(ct);
        });

    public async Task<IReadOnlyList<EKATTE>> GetSettlementsAsync(int municipalityId, CancellationToken ct = default) =>
        await GetOrAddAsync($"settlements:{municipalityId}", async () =>
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            return await db.EKATTE.AsNoTracking()
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
