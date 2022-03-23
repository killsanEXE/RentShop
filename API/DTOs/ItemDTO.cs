using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.DTOs
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PreviewPhotoUrl { get; set; }
        public int PricePerDay { get; set; }
        public int AgeRestriction { get; set; } = 16;
        // public ICollection<ItemUnitPoint>? Units { get; set; }
        public IReadOnlyList<UnitDTO>? Units { get; set; }
        public IReadOnlyList<PhotoDTO>? Photos { get; set; }
    }
}