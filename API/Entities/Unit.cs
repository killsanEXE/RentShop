using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    [Table("Units")]
    public class Unit
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime? WhenWillBeAvailable { get; set; }
        public string? Description { get; set; }
        public ItemUnitPoint? ItemUnitPoint { get; set; }
        public bool Disabled { get; set; }
    }
}