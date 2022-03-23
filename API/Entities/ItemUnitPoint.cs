using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class ItemUnitPoint
    {
        public int Id { get; set; }
        [InverseProperty("Units")]
        public Item? Item { get; set; }
        [InverseProperty("Units")]
        public Point? Point { get; set; }
        public int UnitId { get; set; }
        [ForeignKey("UnitId")]
        public Unit? Unit { get; set; }
    }
}