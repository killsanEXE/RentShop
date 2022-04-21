using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;

namespace API.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmail(EmailMessage message);
    }
}