using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatAPI.Entities
{
    [Table("Locations")]
    public class Location
    {
        public int Id { get; set; }
        public string? Country { get; set; } 
        public string? City { get; set; }
        public string? Address { get; set; }
        public int Floor { get; set; }
        public string? Apartment { get; set; }
    }
}