using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class UnitDTO
    {
        public int Id { get; set; }
        [Required]
        public int PointId { get; set; }
        [Required]
        public string? Description { get; set; }
        public DateTime? WhenWillBeAvaliable { get; set; }
        public bool IsAvaliable { get; set; }
        public PointDTO? Point { get; set; }
        public bool Disabled { get; set; }
    }
}