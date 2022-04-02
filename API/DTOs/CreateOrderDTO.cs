using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class CreateOrderDTO
    {
        [Required]
        public int? UnitId { get; set; }
        public int? DeliveryLocation { get; set; }
        [Required]
        public DateTime DeliveryDate { get; set; }
        [Required]
        public DateTime ReturnDate { get; set; }
    }
}