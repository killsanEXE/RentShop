using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class ClientDTO
    {
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? PhotoUrl { get; set; }
        public int Age { get; set; }
    }
}