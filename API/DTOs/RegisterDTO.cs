using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 6)]
        public string? Password { get; set; }
    }
}