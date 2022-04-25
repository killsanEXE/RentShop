using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class GroupDTO
    {
        public bool UnreadMessages { get; set; }
        public string? LastMessageSender { get; set; }
        public string? LastMessageContent { get; set; }
        public string? Username1 { get; set; }
        public string? Username2 { get; set; }
        public string? User1Photo { get; set; }
        public string? User2Photo { get; set; }
    }
}