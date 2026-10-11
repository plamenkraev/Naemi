namespace NaemZZ.Core.Entities;

/// <summary>
/// Landlord ("Наемодател"). Recreated from the legacy EFDAL "NAEMODATELI" entity
/// (recovered from dbZZ.csdl/.ssdl via decompilation). Field names and types are
/// preserved from the original SQL Server Compact schema.
/// </summary>
public class NAEMODATELI
{
    public int ID { get; set; }

    public string? NAME { get; set; } = "[нов]";

    public string? SURNAME { get; set; }

    public string? LASTNAME { get; set; }

    /// <summary>FK to <see cref="Entities.EKATTE"/> (settlement / Населено място).</summary>
    public int? CITY { get; set; }

    public string? STREET { get; set; }

    public string? EGN { get; set; }

    /// <summary>ID card ("лична карта") number.</summary>
    public string? LK { get; set; }

    /// <summary>ID card issue date.</summary>
    public DateTime? LKDATE { get; set; }

    /// <summary>Issuing police department ("МВР").</summary>
    public string? MVR { get; set; }

    /// <summary>FK to <see cref="Entities.FIRM"/>; selects which contract template to print.</summary>
    public int? FIRM_ID { get; set; }

    /// <summary>FK to <see cref="EK_OBST"/> (Община / municipality).</summary>
    public int? OBSTINA { get; set; }

    /// <summary>FK to <see cref="EK_OBL"/> (Област / region).</summary>
    public int? OBLAST { get; set; }

    public string? Phone { get; set; }

    public FIRM? FIRM { get; set; }

    public EK_OBL? EK_OBL { get; set; }

    public EK_OBST? EK_OBST { get; set; }

    public EKATTE? EKATTE { get; set; }

    public ICollection<IMOTI> IMOTIs { get; set; } = new List<IMOTI>();

    /// <summary>Total area (дка) of all properties belonging to this landlord. Mirrors legacy "GetTotal".</summary>
    public decimal GetTotal => IMOTIs.Sum(i => i.PLOST ?? 0m);
}
