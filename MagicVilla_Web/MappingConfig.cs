using AutoMapper;
using MagicVilla_VillaAPI.Models.DTOs.VillaNumberDTOs;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTOs.VillaDTOs;

namespace MagicVilla_Web;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        // Villa DTOs Mapping
        CreateMap<Villa, CreateVillaDTO>().ReverseMap();
        CreateMap<Villa, UpdateVillaDTO>().ReverseMap();

        // VillaNumbers DTOs Mapping
        CreateMap<VillaNumber, CreateVillaNumberDTO>().ReverseMap();
        CreateMap<VillaNumber, UpdateVillaNumberDTO>().ReverseMap();
    }
}
