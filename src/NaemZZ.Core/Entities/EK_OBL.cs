namespace NaemZZ.Core.Entities;

/// <summary>Region lookup ("Област").</summary>
public class EK_OBL
{
    public int ID { get; set; }

    public string? OBLAST { get; set; }

    public string? EKATTE { get; set; }

    public string? NAME { get; set; }

    public ICollection<EK_OBST> EK_OBSTs { get; set; } = new List<EK_OBST>();

    public ICollection<IMOTI> IMOTIs { get; set; } = new List<IMOTI>();

    public ICollection<NAEMODATELI> NAEMODATELIs { get; set; } = new List<NAEMODATELI>();

    public override string ToString() => $"{EKATTE} | {NAME}";
}
