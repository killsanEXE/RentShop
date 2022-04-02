using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public UnitDTO? Unit { get; set; }
        public ClientDTO? Client { get; set; }
        public ClientDTO? DeliveryMan { get; set; }
        public LocationDTO? DeliveryLocation { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool DeliveryInProcess { get; set; } 
        public bool Cancelled { get; set; }
        public bool DeliveryCompleted { get; set; }
        public bool ClientGotDelivery { get; set; }
        public bool InUsage { get; set; }
    }
}