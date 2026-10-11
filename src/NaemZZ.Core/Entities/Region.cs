namespace NaemZZ.Core.Entities;

/// <summary>Region lookup ("Област").</summary>
public class Region
{
    public int ID { get; set; }

    public string? OBLAST { get; set; }

    public string? Settlement { get; set; }

    public string? NAME { get; set; }

    public ICollection<Municipality> EK_OBSTs { get; set; } = new List<Municipality>();

    public ICollection<Property> IMOTIs { get; set; } = new List<Property>();

    public ICollection<Landlord> NAEMODATELIs { get; set; } = new List<Landlord>();

    public override string ToString() => $"{Settlement} | {NAME}";
}
