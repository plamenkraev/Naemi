namespace NaemZZ.Core.Entities;

/// <summary>Firm ("Фирма"); selects which of the three rental-contract templates is printed.</summary>
public class FIRM
{
    public int firmID { get; set; }

    public string? firmNAME { get; set; }

    public string? firmADDRESS { get; set; }

    /// <summary>Bulgarian company registration number ("БУЛСТАТ").</summary>
    public string? BULSTAT { get; set; }

    /// <summary>Managing representative ("МОЛ - Материално отговорно лице").</summary>
    public string? firmMOL { get; set; }

    public string? firmEGN { get; set; }

    public string? firmLK { get; set; }

    public DateTime? firmLKDATE { get; set; }

    public string? firmMVR { get; set; }

    public int? ContractTemplateId { get; set; }

    public ContractTemplate? ContractTemplate { get; set; }

    public ICollection<NAEMODATELI> NAEMODATELIs { get; set; } = new List<NAEMODATELI>();
}
