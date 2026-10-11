namespace NaemZZ.Core.Entities;

/// <summary>
/// Landlord ("Наемодател"). Recreated from the legacy EFDAL "Landlord" entity
/// (recovered from dbZZ.csdl/.ssdl via decompilation). Field names and types are
/// preserved from the original SQL Server Compact schema.
/// </summary>
public class Landlord
{
    public int ID { get; set; }

    public bool IsCompany { get; set; }
    public string? CompanyName { get; set; }
    public string? EIK { get; set; }
    public string? MOL { get; set; }

    public string? NAME { get; set; } = "[нов]";

    public string? SURNAME { get; set; }

    public string? LASTNAME { get; set; }

    /// <summary>FK to <see cref="Entities.Settlement"/> (settlement / Населено място).</summary>
    public int? CITY { get; set; }

    public string? STREET { get; set; }

    public string? EGN { get; set; }

    /// <summary>ID card ("лична карта") number.</summary>
    public string? LK { get; set; }

    /// <summary>ID card issue date.</summary>
    public DateTime? LKDATE { get; set; }

    /// <summary>Issuing police department ("МВР").</summary>
    public string? MVR { get; set; }

    /// <summary>FK to <see cref="Entities.Company"/>; selects which contract template to print.</summary>
    public int? FIRM_ID { get; set; }

    /// <summary>FK to <see cref="Municipality"/> (Община / municipality).</summary>
    public int? OBSTINA { get; set; }

    /// <summary>FK to <see cref="Region"/> (Област / region).</summary>
    public int? OBLAST { get; set; }

    public string? Phone { get; set; }

    /// <summary>Optional contract template override; null uses the selected company template.</summary>
    public int? ContractTemplateId { get; set; }
    public ContractTemplate? ContractTemplate { get; set; }

    public Company? Company { get; set; }

    public Region? Region { get; set; }

    public Municipality? Municipality { get; set; }

    public Settlement? Settlement { get; set; }

    public ICollection<Property> IMOTIs { get; set; } = new List<Property>();

    /// <summary>Total area (дка) of all properties belonging to this landlord. Mirrors legacy "GetTotal".</summary>
    public decimal GetTotal => IMOTIs.Sum(i => i.PLOST ?? 0m);
}
