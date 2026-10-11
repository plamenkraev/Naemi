namespace NaemZZ.Core.Entities;

/// <summary>Property category lookup ("Категория").</summary>
public class CATEGORI
{
    public int ID { get; set; }

    public string? NAME { get; set; }

    public ICollection<IMOTI> IMOTIs { get; set; } = new List<IMOTI>();

    public override string ToString() => NAME ?? string.Empty;
}
