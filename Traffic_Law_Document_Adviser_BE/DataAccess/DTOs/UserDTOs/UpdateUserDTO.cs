using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [RegularExpression("^(User|Expert|Admin)$", ErrorMessage = "Role must be User, Expert, or Admin.")]
        public string? Role { get; set; }

        public bool? IsActive { get; set; }
    }
}
