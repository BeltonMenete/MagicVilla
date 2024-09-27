using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTOs;

namespace MagicVilla_VillaAPI.Mappers
{
    public static class VillaMapper
    {
        public static Villa ToVillaFromCreate(this CreateVillaDTO villaDTO)
        {
            return new()
            {
                Name = villaDTO.Name,
                Details = villaDTO.Details,
                Rate = villaDTO.Rate,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft,
                ImageUrl = villaDTO.ImageUrl,
                Amenity = villaDTO.Amenity,
            };
        }
        public static Villa ToVillaFromUpdate(this UpdateVillaDTO villaDTO)
        {
            return new()
            {
                Id = villaDTO.Id,
                Name = villaDTO.Name,
                Details = villaDTO.Details,
                Rate = villaDTO.Rate,
                Occupancy = villaDTO.Occupancy,
                Sqft = villaDTO.Sqft,
                ImageUrl = villaDTO.ImageUrl,
                Amenity = villaDTO.Amenity,
            };
        }
        public static UpdateVillaDTO ToUpdateVillaDTO(this Villa villa)
        {
            return new()
            {
                Id = villa.Id,
                Name = villa.Name,
                Details = villa.Details,
                Rate = villa.Rate,
                Occupancy = villa.Occupancy,
                Sqft = villa.Sqft,
                ImageUrl = villa.ImageUrl,
                Amenity = villa.Amenity,
            };
        }
    }
}