using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class UserDTO
    {
        public string? Username { get; set; }
        public string? Token { get; set; }
        public string? PhotoUrl { get; set; }
        public bool DeliverymanRequest { get; set; }
        public List<LocationDTO>? Locations { get; set; }
    }
}