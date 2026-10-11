namespace NaemZZ.Core.Entities;

/// <summary>
/// Property ("Имот") belonging to a landlord. Recreated from the legacy EFDAL "IMOTI" entity.
/// </summary>
public class IMOTI
{
    public int imotiID { get; set; }

    /// <summary>FK to <see cref="EKATTE"/> (Населено място / settlement).</summary>
    public int? EKATTE_CITY { get; set; }

    /// <summary>FK to <see cref="EK_OBST"/> (Община / municipality).</summary>
    public int? EKATTE_MUNICIPALITY { get; set; }

    /// <summary>FK to <see cref="EK_OBL"/> (Област / region).</summary>
    public int? EKATTE_AREA { get; set; }

    /// <summary>Area in дка (decare). Maps to SQL Server Compact "numeric(scale=3)".</summary>
    public decimal? PLOST { get; set; }

    /// <summary>FK to <see cref="CATEGORI"/>.</summary>
    public int? CATEGORY { get; set; }

    /// <summary>Terrain / locality name ("Местност").</summary>
    public string? TERRAIN { get; set; }

    /// <summary>Cadastral number ("Номер"). Required, digits only, max length 6 (legacy validation rule).</summary>
    public string? NOMER { get; set; }

    /// <summary>Borders ("Граници").</summary>
    public string? LIMITS { get; set; }

    /// <summary>FK to the owning <see cref="NAEMODATELI"/>.</summary>
    public int? nID { get; set; }

    public CATEGORI? CATEGORI { get; set; }

    public EK_OBL? EK_OBL { get; set; }

    public EK_OBST? EK_OBST { get; set; }

    public EKATTE? EKATTE { get; set; }

    public NAEMODATELI? NAEMODATELI { get; set; }
}
