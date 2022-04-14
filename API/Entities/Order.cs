using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public Unit? Unit { get; set; }
        public AppUser? Client { get; set; }
        public AppUser? DeliveryMan { get; set; }
        public Location? DeliveryLocation { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool DeliveryInProcess { get; set; } 
        public bool Cancelled { get; set; }
        public bool DeliveryCompleted { get; set; }
        public bool ClientGotDelivery { get; set; }
        
        public bool InUsage { get; set; }

        public AppUser? ReturnDeliveryman { get; set; }
        public bool UnitReturned { get; set; }
        public Location? ReturnFromLocation { get; set; }
        public Point? ReturnPoint { get; set; }
    }
}