namespace NaemZZ.Core.Entities;

/// <summary>Municipality lookup ("Община"), cascades from <see cref="Region"/>.</summary>
public class Municipality
{
    public int ID { get; set; }

    public string? Settlement { get; set; }

    public string? NAME { get; set; }

    /// <summary>FK to the parent <see cref="Region"/> (region).</summary>
    public int? OBL_ID { get; set; }

    public Region? Region { get; set; }

    public ICollection<Settlement> EKATTEs { get; set; } = new List<Settlement>();

    public ICollection<Property> IMOTIs { get; set; } = new List<Property>();

    public ICollection<Landlord> NAEMODATELIs { get; set; } = new List<Landlord>();

    public override string ToString() => $"{Settlement} | {NAME}";
}
