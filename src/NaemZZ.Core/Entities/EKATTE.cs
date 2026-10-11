namespace NaemZZ.Core.Entities;

/// <summary>
/// Settlement lookup ("Населено място"), cascades from <see cref="EK_OBST"/>.
/// </summary>
/// <remarks>
/// The legacy SQL Server Compact column is named "EKATTE", same as the table/entity name.
/// The decompiled EFDAL class renamed the scalar property to "EKATTE1" to avoid a C# naming
/// collision; here it is renamed to <see cref="EKATTE_CODE"/> for clarity and mapped back to
/// the original "EKATTE" column name via Fluent API configuration.
/// </remarks>
public class EKATTE
{
    public int ID { get; set; }

    /// <summary>EKATTE cadastral code. Maps to the legacy "EKATTE" column.</summary>
    public string? EKATTE_CODE { get; set; }

    /// <summary>Settlement type abbreviation ("с./гр." etc.).</summary>
    public string? T_V_M { get; set; }

    public string? NAME { get; set; }

    /// <summary>FK to the parent <see cref="EK_OBST"/> (municipality).</summary>
    public int? OBST_ID { get; set; }

    public EK_OBST? EK_OBST { get; set; }

    public ICollection<IMOTI> IMOTIs { get; set; } = new List<IMOTI>();

    public ICollection<NAEMODATELI> NAEMODATELIs { get; set; } = new List<NAEMODATELI>();

    public override string ToString() => $"{EKATTE_CODE} | {NAME}";
}
