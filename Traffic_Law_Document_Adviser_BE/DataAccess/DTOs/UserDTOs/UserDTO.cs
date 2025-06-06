using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs.UserDTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!; // “User”, “Expert”, or “Admin”
        public bool IsActive { get; set; }
    }
}
