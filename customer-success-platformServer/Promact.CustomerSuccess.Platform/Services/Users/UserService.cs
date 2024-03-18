using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Promact.CustomerSuccess.Platform.Entities;
using Promact.CustomerSuccess.Platform.Services.Dtos;
using Promact.CustomerSuccess.Platform.Services.Dtos.Auth;
using Promact.CustomerSuccess.Platform.Services.Dtos.Auth.Auth;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Promact.CustomerSuccess.Platform.Services.Users
{
    public class UserService :  CrudAppService<User, UserDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateUserDto, CreateUpdateUserDto>, IUserService
    {
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<UserRole, Guid> _userRoleRepository;
        private readonly IRepository<Role, Guid> _roleRepository;
        private readonly IMapper _mapper;

        public UserService(
            IRepository<User, Guid> userRepository,
            IRepository<UserRole, Guid> userRoleRepository,
            IRepository<Role, Guid> roleRepository,
            IMapper mapper):base(userRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> GetUserByUsernameAndEmailAsync(string username, string email)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.UserName == username && u.Email == email);

            if (user == null)
            {
                // Handle if user not found
                return null;
            }

            var userDto = _mapper.Map<User, UserDto>(user);

            // Fetch the UserRole
            var userRole = await _userRoleRepository.FirstOrDefaultAsync(ur => ur.UserId == user.Id);

            if (userRole != null)
            {
                // Fetch the associated Role
                var role = await _roleRepository.GetAsync(userRole.RoleId);

                // Map RoleDto
                userDto.Role = _mapper.Map<Role, RoleDto>(role);
            }

            return userDto;
        }

        public async Task<UserDto> GetDetailByEmailAsync(string email)
        {
            var user = (await _userRepository.GetListAsync(u => u.Email == email)).FirstOrDefault();
            return ObjectMapper.Map<User, UserDto>(user);
        }

        public async Task<List<UserDto>> GetManagersAsync()
        {
            // Fetch the manager role
            var managerRole = await _roleRepository.FirstOrDefaultAsync(r => r.Name == "Manager");

            if (managerRole == null)
            {
                // Handle if manager role not found
                return null;
            }

            // Fetch users with manager role
            var managerUserRoles = await _userRoleRepository.GetListAsync(ur => ur.RoleId == managerRole.Id);
            var managerUserIds = managerUserRoles.Select(ur => ur.UserId);

            // Fetch manager users
            var managerUsers = await _userRepository.GetListAsync(u => managerUserIds.Contains(u.Id));

            // Map manager users to UserDto
            var managerUserDtos = managerUsers.Select(user => _mapper.Map<User, UserDto>(user)).ToList();

            // You may also want to include additional information such as manager's role, etc.

            return managerUserDtos;
        }

    }
}
