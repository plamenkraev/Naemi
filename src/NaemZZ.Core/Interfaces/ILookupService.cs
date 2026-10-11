using NaemZZ.Core.Entities;

namespace NaemZZ.Core.Interfaces;

/// <summary>Read-only access to the small lookup/reference tables (cached, rarely change).</summary>
public interface ILookupService
{
    Task<IReadOnlyList<FIRM>> GetFirmsAsync(CancellationToken ct = default);

    Task<IReadOnlyList<CATEGORI>> GetCategoriesAsync(CancellationToken ct = default);

    Task<IReadOnlyList<EK_OBL>> GetRegionsAsync(CancellationToken ct = default);

    Task<IReadOnlyList<EK_OBST>> GetMunicipalitiesAsync(int regionId, CancellationToken ct = default);

    Task<IReadOnlyList<EKATTE>> GetSettlementsAsync(int municipalityId, CancellationToken ct = default);

    /// <summary>Forces a refresh of the in-memory lookup cache (lookups rarely change, so they are cached).</summary>
    void InvalidateCache();
}
