using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Token { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}