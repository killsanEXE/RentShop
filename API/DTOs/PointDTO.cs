using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class PointDTO : LocationDTO
    {
        public string? PhotoUrl { get; set; }
        public bool Disabled { get; set; }
    }
}