namespace NaemZZ.Core.Entities;

/// <summary>Editable HTML contract template. Original RTF files remain untouched.</summary>
public class ContractTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Html { get; set; } = "";
    public string? LegacyRtfFile { get; set; }
    public ICollection<FIRM> Firms { get; set; } = new List<FIRM>();
}
