using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Point : Location
    {
        public string? PhotoUrl { get; set; }
        [InverseProperty(nameof(ItemUnitPoint.Point))]
        public ICollection<ItemUnitPoint>? Units { get; set; }
        public string? PublicPhotoId { get; set; }
        public bool Disabled { get; set; }
    }
}