namespace NaemZZ.Core.Dto;

public sealed class LandlordDto
{
    public int ID { get; set; }
    public string? NAME { get; set; } = "[нов]";
    public string? SURNAME { get; set; }
    public string? LASTNAME { get; set; }
    public int? CITY { get; set; }
    public string? STREET { get; set; }
    public string? EGN { get; set; }
    public string? LK { get; set; }
    public DateTime? LKDATE { get; set; }
    public string? MVR { get; set; }
    public int? FIRM_ID { get; set; }
    public int? OBSTINA { get; set; }
    public int? OBLAST { get; set; }
    public string? Phone { get; set; }
    public List<PropertyDto> IMOTIs { get; set; } = [];
    public decimal GetTotal => IMOTIs.Sum(x => x.PLOST ?? 0m);
}

public sealed class PropertyDto
{
    public int imotiID { get; set; }
    public int? EKATTE_CITY { get; set; }
    public int? EKATTE_MUNICIPALITY { get; set; }
    public int? EKATTE_AREA { get; set; }
    public decimal? PLOST { get; set; }
    public int? CATEGORY { get; set; }
    public string? TERRAIN { get; set; }
    public string? NOMER { get; set; }
    public string? LIMITS { get; set; }
    public int? nID { get; set; }
}

public sealed class FirmDto
{
    public int firmID { get; set; }
    public string? firmNAME { get; set; }
    public string? firmADDRESS { get; set; }
    public string? BULSTAT { get; set; }
    public string? firmMOL { get; set; }
    public string? firmEGN { get; set; }
    public string? firmLK { get; set; }
    public DateTime? firmLKDATE { get; set; }
    public string? firmMVR { get; set; }
    public int? ContractTemplateId { get; set; }
}
