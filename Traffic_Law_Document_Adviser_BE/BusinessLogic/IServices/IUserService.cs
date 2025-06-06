using DataAccess.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(Guid id);
        Task<UserDTO> CreateUserAsync(CreateUserDTO dto);
        Task<UserDTO> UpdateUserAsync(UpdateUserDTO dto);
        Task DeleteUserAsync(Guid id);
    }
}
