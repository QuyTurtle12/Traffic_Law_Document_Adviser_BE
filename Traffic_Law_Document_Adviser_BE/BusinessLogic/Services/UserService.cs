using AutoMapper;
using BusinessLogic.IServices;
using DataAccess.Constant;
using DataAccess.DTOs.UserDTOs;
using DataAccess.Entities;
using DataAccess.ExceptionCustom;
using DataAccess.IRepositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUOW _uow;
        private readonly IMapper _mapper;

        public UserService(IUOW uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var repo = _uow.GetRepository<User>();
            var users = await repo.GetAllAsync();
            return users.Select(u => _mapper.Map<UserDTO>(u));
        }

        public async Task<UserDTO> GetUserByIdAsync(Guid id)
        {
            var user = await _uow.GetRepository<User>().GetByIdAsync(id);
            if (user == null)
                throw new ErrorException(
                  StatusCodes.Status404NotFound,
                  "USER_NOT_FOUND",
                  $"No user found with ID={id}"
                );
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> CreateUserAsync(CreateUserDTO dto)
        {
            var repo = _uow.GetRepository<User>();

            // 1. Duplicate email?
            if (repo.Entities.Any(u => u.Email == dto.Email))
                throw new ErrorException(
                  StatusCodes.Status400BadRequest,
                  "EMAIL_EXISTS",
                  "Email is already registered."
                );

            // 2. Map & insert
            var userEntity = _mapper.Map<User>(dto);
            await repo.InsertAsync(userEntity);
            await _uow.SaveAsync();

            return _mapper.Map<UserDTO>(userEntity);
        }

        public async Task<UserDTO> UpdateUserAsync(UpdateUserDTO dto)
        {
            var repo = _uow.GetRepository<User>();
            var user = await repo.GetByIdAsync(dto.Id);
            if (user == null)
                throw new ErrorException(
                  StatusCodes.Status404NotFound,
                  "USER_NOT_FOUND",
                  $"No user found with ID={dto.Id}"
                );

            // If email changed, check uniqueness
            if (!string.IsNullOrWhiteSpace(dto.Email)
                && dto.Email != user.Email
                && repo.Entities.Any(u => u.Email == dto.Email && u.Id != dto.Id))
            {
                throw new ErrorException(
                  StatusCodes.Status400BadRequest,
                  "EMAIL_EXISTS",
                  "Email is already in use by another account."
                );
            }

            // If role changed, convert to int
            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                user.Role = RoleConstants.ToRoleValue(dto.Role);
            }

            // Update other non-null properties via AutoMapper
            _mapper.Map(dto, user);

            // Save
            repo.Update(user);
            await _uow.SaveAsync();

            return _mapper.Map<UserDTO>(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var repo = _uow.GetRepository<User>();
            var user = await repo.GetByIdAsync(id);
            if (user == null)
                throw new ErrorException(
                  StatusCodes.Status404NotFound,
                  "USER_NOT_FOUND",
                  $"No user found with ID={id}"
                );

            repo.Delete(user);
            await _uow.SaveAsync();
        }
    }
}
