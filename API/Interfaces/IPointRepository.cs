using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IPointRepository
    {
        Task<Point> GetPointByIdAsync(int id);
        void AddPoint(Point point);
        Task<IEnumerable<PointDTO>> GetDTOPointsAsync();
        Task<PointDTO> GetPointDTO(int id);
    }
}