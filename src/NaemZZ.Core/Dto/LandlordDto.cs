namespace NaemZZ.Core.Dto;

public sealed class LandlordListDto
{
    public int ID { get; set; }
    public string? NAME { get; set; }
    public string? SURNAME { get; set; }
    public string? LASTNAME { get; set; }
    public int? CompanyId { get; set; }
    public int PropertyCount { get; set; }
    public bool IsCompany { get; set; }
    public string? LegalName { get; set; }
    public string? CompanyName { get; set; }
    public string? EIK { get; set; }
    public string? MOL { get; set; }
    public string? Email { get; set; }
}

public sealed class LandlordDto
{
    public int ID { get; set; }
    public bool IsCompany { get; set; }
    public string? CompanyName { get; set; }
    public decimal? DefaultPrice { get; set; }
    public string? EIK { get; set; }
    public string? MOL { get; set; }
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
    public int? ContractTemplateId { get; set; }
    public int? OBSTINA { get; set; }
    public int? OBLAST { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal? Price { get; set; }
    public List<PropertyDto> IMOTIs { get; set; } = [];
    public decimal GetTotalArea => IMOTIs.Sum(x => x.PLOST ?? 0m);
    public string? GetFullName()
    {
        if (IsCompany)
        {
            return CompanyName;
        }
        else
        {
            return $"{NAME} {SURNAME} {LASTNAME}".Trim();
        }
    }
}

public sealed class PropertyDto
{
    public int Id { get; set; }
    public int? EKATTE_CITY { get; set; }
    public int? EKATTE_MUNICIPALITY { get; set; }
    public int? EKATTE_AREA { get; set; }
    public decimal? PLOST { get; set; }
    public int? CATEGORY { get; set; }
    public string? TERRAIN { get; set; }
    public string? NOMER { get; set; }
    public string? LIMITS { get; set; }
    public int? LandlordId { get; set; }
}

public sealed class FirmDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? BULSTAT { get; set; }
    public string? Representative { get; set; }
    public string? RepresentativeEgn { get; set; }
    public string? RepresentativeIdCard { get; set; }
    public DateTime? RepresentativeIdCardDate { get; set; }
    public string? RepresentativeIdCardIssuer { get; set; }
    public int? ContractTemplateId { get; set; }
    public decimal? Price { get; set; }
    public int LandlordCount { get; set; }
    public int PropertyCount { get; set; }
    public decimal PropertyAmount { get; set; }
    public decimal TotalPrice { get; set; }
}
