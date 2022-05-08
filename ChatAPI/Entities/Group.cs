using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatAPI.Entities
{
    public class Group
    {
        public Group()
        {
        }

        public Group(string? name)
        {
            Name = name;
        }

        [Key]
        public string? Name { get; set; }
        public ICollection<Connection>? Connections { get; set; } = new List<Connection>();
        public bool UnreadMessages { get; set; }
        public string? LastMessageSender { get; set; }
        public string? LastMessageContent { get; set; }
        public AppUser? User1 { get; set; }
        public AppUser? User2 { get; set; }
    }
}