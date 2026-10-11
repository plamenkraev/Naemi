namespace NaemZZ.Core.Entities;

/// <summary>
/// Settlement lookup ("Населено място"), cascades from <see cref="Municipality"/>.
/// </summary>
/// <remarks>
/// The legacy SQL Server Compact column is named "Settlement", same as the table/entity name.
/// The decompiled EFDAL class renamed the scalar property to "EKATTE1" to avoid a C# naming
/// collision; here it is renamed to <see cref="EKATTE_CODE"/> for clarity and mapped back to
/// the original "Settlement" column name via Fluent API configuration.
/// </remarks>
public class Settlement
{
    public int ID { get; set; }

    /// <summary>Settlement cadastral code. Maps to the legacy "Settlement" column.</summary>
    public string? EKATTE_CODE { get; set; }

    /// <summary>Settlement type abbreviation ("с./гр." etc.).</summary>
    public string? T_V_M { get; set; }

    public string? NAME { get; set; }

    /// <summary>FK to the parent <see cref="Municipality"/> (municipality).</summary>
    public int? OBST_ID { get; set; }

    public Municipality? Municipality { get; set; }

    public ICollection<Property> IMOTIs { get; set; } = new List<Property>();

    public ICollection<Landlord> NAEMODATELIs { get; set; } = new List<Landlord>();

    public override string ToString() => $"{EKATTE_CODE} | {NAME}";
}
