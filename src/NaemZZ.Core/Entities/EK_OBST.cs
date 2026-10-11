namespace NaemZZ.Core.Entities;

/// <summary>Municipality lookup ("Община"), cascades from <see cref="EK_OBL"/>.</summary>
public class EK_OBST
{
    public int ID { get; set; }

    public string? EKATTE { get; set; }

    public string? NAME { get; set; }

    /// <summary>FK to the parent <see cref="EK_OBL"/> (region).</summary>
    public int? OBL_ID { get; set; }

    public EK_OBL? EK_OBL { get; set; }

    public ICollection<Entities.EKATTE> EKATTEs { get; set; } = new List<Entities.EKATTE>();

    public ICollection<IMOTI> IMOTIs { get; set; } = new List<IMOTI>();

    public ICollection<NAEMODATELI> NAEMODATELIs { get; set; } = new List<NAEMODATELI>();

    public override string ToString() => $"{EKATTE} | {NAME}";
}
