using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class EmailMessage
    {
        public EmailMessage() {}
        public EmailMessage(string email, string title, string message)
        {
            Email = email;
            Title = title;
            Message = message;
        }
        public string Email { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}