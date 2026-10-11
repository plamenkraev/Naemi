namespace NaemZZ.Core.Entities;

/// <summary>Property category lookup ("Категория").</summary>
public class PropertyCategory
{
    public int ID { get; set; }

    public string? NAME { get; set; }

    public ICollection<Property> IMOTIs { get; set; } = new List<Property>();

    public override string ToString() => NAME ?? string.Empty;
}
