using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.UserDTOs
{
    public class CreateUserDTO
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(
          @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
          ErrorMessage = "Password must contain uppercase, lowercase, digit, and special character."
        )]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        [RegularExpression("^(User|Expert|Admin)$", ErrorMessage = "Role must be User, Expert, or Admin.")]
        public string Role { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
