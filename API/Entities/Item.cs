using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int PricePerDay { get; set; }
        public int AgeRestriction { get; set; } = 16;
        [InverseProperty(nameof(ItemUnitPoint.Item))]
        public ICollection<ItemUnitPoint>? Units { get; set; }
        public ICollection<Photo>? Photos { get; set; }
        public Photo? PreviewPhoto { get; set; }
    }
}