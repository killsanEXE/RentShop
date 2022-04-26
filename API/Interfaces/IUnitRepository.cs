using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface IUnitRepository
    {
        Task<Unit> GetUnitByIdAsync(int id);
        void AddUnit(Item item, Point point, Unit unit);
    }
}