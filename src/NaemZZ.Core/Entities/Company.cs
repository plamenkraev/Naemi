namespace NaemZZ.Core.Entities;

/// <summary>Firm ("Фирма"); selects which of the three rental-contract templates is printed.</summary>
public class Company
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    /// <summary>Bulgarian company registration number ("БУЛСТАТ").</summary>
    public string? BULSTAT { get; set; }

    /// <summary>Managing representative ("МОЛ - Материално отговорно лице").</summary>
    public string? Representative { get; set; }

    public string? RepresentativeEgn { get; set; }

    public string? RepresentativeIdCard { get; set; }

    public DateTime? RepresentativeIdCardDate { get; set; }

    public string? RepresentativeIdCardIssuer { get; set; }

    public int? ContractTemplateId { get; set; }

    public ContractTemplate? ContractTemplate { get; set; }

    public ICollection<Landlord> NAEMODATELIs { get; set; } = new List<Landlord>();
}
