using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string? Name { get; set; }
        public string? PhotoUrl { get; set; }
        public string? PublicPhotoId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? CreditCard { get; set; }
        ICollection<Location>? Locations { get; set; }
        public ICollection<AppUserRole>? UserRoles { get; set; }
    }
}