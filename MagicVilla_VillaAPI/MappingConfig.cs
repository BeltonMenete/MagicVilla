using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTOs;
using MagicVilla_VillaAPI.Models.DTOs.VillaDTOs;
using MagicVilla_VillaAPI.Models.DTOs.VillaNumberDTOs;

namespace MagicVilla_VillaAPI;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        // Villas DTOS mapping
        CreateMap<Villa, CreateVillaDTO>().ReverseMap();
        CreateMap<Villa, UpdateVillaDTO>().ReverseMap();

        // VillaNumbers DTOs mapping
        CreateMap<VillaNumber, CreateVillaNumberDTO>().ReverseMap();
        CreateMap<VillaNumber, UpdateVillaNumberDTO>().ReverseMap();
    }
}


