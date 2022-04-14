using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public AppUser? User { get; set; }
        public bool UnreadMessages { get; set; }
    }
}