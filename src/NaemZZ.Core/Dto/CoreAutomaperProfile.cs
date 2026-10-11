using AutoMapper;
using NaemZZ.Core.Entities;

namespace NaemZZ.Core.Dto;

public sealed class CoreAutomaperProfile : Profile
{
    public CoreAutomaperProfile()
    {
        CreateMap<NAEMODATELI, LandlordDto>();
        CreateMap<LandlordDto, NAEMODATELI>()
            .ForMember(d => d.IMOTIs, o => o.Ignore())
            .ForMember(d => d.FIRM, o => o.Ignore())
            .ForMember(d => d.EK_OBL, o => o.Ignore())
            .ForMember(d => d.EK_OBST, o => o.Ignore())
            .ForMember(d => d.EKATTE, o => o.Ignore());
        CreateMap<LandlordDto, LandlordDto>();
        CreateMap<IMOTI, PropertyDto>();
        CreateMap<PropertyDto, IMOTI>()
            .ForMember(d => d.NAEMODATELI, o => o.Ignore())
            .ForMember(d => d.CATEGORI, o => o.Ignore())
            .ForMember(d => d.EK_OBL, o => o.Ignore())
            .ForMember(d => d.EK_OBST, o => o.Ignore())
            .ForMember(d => d.EKATTE, o => o.Ignore());
        CreateMap<PropertyDto, PropertyDto>();
        CreateMap<FIRM, FirmDto>();
        CreateMap<FirmDto, FIRM>()
            .ForMember(d => d.ContractTemplate, o => o.Ignore())
            .ForMember(d => d.NAEMODATELIs, o => o.Ignore());
        CreateMap<FirmDto, FirmDto>();
    }
}
