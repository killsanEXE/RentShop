using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IItemRepository ItemRepository { get; }
        IOrderRepository OrderRepository { get; }
        IUnitRepository UnitRepository { get; }
        IPointRepository PointRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}