namespace NaemZZ.Core.Entities;

/// <summary>
/// Property ("Имот") belonging to a landlord. Recreated from the legacy EFDAL "Property" entity.
/// </summary>
public class Property
{
    public int Id { get; set; }

    /// <summary>FK to <see cref="Settlement"/> (Населено място / settlement).</summary>
    public int? EKATTE_CITY { get; set; }

    /// <summary>FK to <see cref="Municipality"/> (Община / municipality).</summary>
    public int? EKATTE_MUNICIPALITY { get; set; }

    /// <summary>FK to <see cref="Region"/> (Област / region).</summary>
    public int? EKATTE_AREA { get; set; }

    /// <summary>Area in дка (decare). Maps to SQL Server Compact "numeric(scale=3)".</summary>
    public decimal? PLOST { get; set; }

    /// <summary>FK to <see cref="PropertyCategory"/>.</summary>
    public int? CATEGORY { get; set; }

    /// <summary>Terrain / locality name ("Местност").</summary>
    public string? TERRAIN { get; set; }

    /// <summary>Cadastral number ("Номер"). Required, digits only, max length 6 (legacy validation rule).</summary>
    public string? NOMER { get; set; }

    /// <summary>Borders ("Граници").</summary>
    public string? LIMITS { get; set; }

    /// <summary>FK to the owning <see cref="Landlord"/>.</summary>
    public int? LandlordId { get; set; }

    public PropertyCategory? PropertyCategory { get; set; }

    public Region? Region { get; set; }

    public Municipality? Municipality { get; set; }

    public Settlement? Settlement { get; set; }

    public Landlord? Landlord { get; set; }
}
