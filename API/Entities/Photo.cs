using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public string? Url { get; set; }
        public bool IsPreview { get; set; }
        public string? PublicId { get; set; }
        public Item? Item { get; set; }
        public int ItemId { get; set; }
    }
}