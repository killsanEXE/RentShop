using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.DTOs
{
    public class DeliverymanDTO : ClientDTO
    {
        public Location? Location { get; set; }
    }
}