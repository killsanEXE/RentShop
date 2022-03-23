using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class LocationDTO
    {
        public int Id { get; set; }
        [Required]
        public string? Country { get; set; } 
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Address { get; set; }
        public int Floor { get; set; }
        public string? Apartment { get; set; }
    }
}