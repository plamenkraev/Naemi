using AutoMapper;
using NaemZZ.Core.Entities;

namespace NaemZZ.Core.Dto;

public sealed class CoreAutomaperProfile : Profile
{
    public CoreAutomaperProfile()
    {
        CreateMap<Landlord, LandlordDto>();
        CreateMap<LandlordDto, Landlord>()
            .ForMember(d => d.IMOTIs, o => o.Ignore())
            .ForMember(d => d.Company, o => o.Ignore())
            .ForMember(d => d.ContractTemplate, o => o.Ignore())
            .ForMember(d => d.Region, o => o.Ignore())
            .ForMember(d => d.Municipality, o => o.Ignore())
            .ForMember(d => d.Settlement, o => o.Ignore());
        CreateMap<LandlordDto, LandlordDto>();
        CreateMap<Property, PropertyDto>();
        CreateMap<PropertyDto, Property>()
            .ForMember(d => d.Landlord, o => o.Ignore())
            .ForMember(d => d.PropertyCategory, o => o.Ignore())
            .ForMember(d => d.Region, o => o.Ignore())
            .ForMember(d => d.Municipality, o => o.Ignore())
            .ForMember(d => d.Settlement, o => o.Ignore());
        CreateMap<PropertyDto, PropertyDto>();
        CreateMap<Company, FirmDto>();
        CreateMap<FirmDto, Company>()
            .ForMember(d => d.ContractTemplate, o => o.Ignore())
            .ForMember(d => d.NAEMODATELIs, o => o.Ignore());
        CreateMap<FirmDto, FirmDto>();
    }
}
