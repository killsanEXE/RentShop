using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class DatasetItemDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int PricePerDay { get; set; }
        public int AgeRestriction { get; set; } = 16;
        public DatasetPhotoDTO? PreviewPhoto { get; set; }
        public IReadOnlyList<DatasetPhotoDTO>? Photos { get; set; }
    }
}