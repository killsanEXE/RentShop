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
        public bool DeliveryCompleted { get; set; }
        public bool ClientGotDelivery { get; set; }
        public bool InUsage { get; set; }
    }
}