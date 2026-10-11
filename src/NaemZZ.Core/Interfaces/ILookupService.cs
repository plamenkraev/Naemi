using NaemZZ.Core.Entities;

namespace NaemZZ.Core.Interfaces;

/// <summary>Read-only access to the small lookup/reference tables (cached, rarely change).</summary>
public interface ILookupService
{
    Task<IReadOnlyList<Company>> GetFirmsAsync(CancellationToken ct = default);

    Task<IReadOnlyList<PropertyCategory>> GetCategoriesAsync(CancellationToken ct = default);

    Task<IReadOnlyList<Region>> GetRegionsAsync(CancellationToken ct = default);

    Task<IReadOnlyList<Municipality>> GetMunicipalitiesAsync(int regionId, CancellationToken ct = default);

    Task<IReadOnlyList<Settlement>> GetSettlementsAsync(int municipalityId, CancellationToken ct = default);

    /// <summary>Forces a refresh of the in-memory lookup cache (lookups rarely change, so they are cached).</summary>
    void InvalidateCache();
}
