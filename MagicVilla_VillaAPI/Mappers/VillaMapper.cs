using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.DTOs;
using MagicVilla_VillaAPI.Models;

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