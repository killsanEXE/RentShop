using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class ReturnUnitDTO
    {
        public int Id { get; set; }
        public string? ReturnFromLocation { get; set; }
        public string? ReturnPoint { get; set; }
    }
}