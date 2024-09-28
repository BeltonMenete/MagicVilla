using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTOs
{
    public class UpdateVillaDTO
    {
        public required int Id { get; set; }
        [MaxLength(30)]
        public required string Name { get; set; }
        public string Details { get; set; } = string.Empty;
        public required double Rate { get; set; }
        public required int Occupancy { get; set; }
        public required int Sqft { get; set; }
        public required string ImageUrl { get; set; }
        public string Amenity { get; set; } = string.Empty;

    }
}