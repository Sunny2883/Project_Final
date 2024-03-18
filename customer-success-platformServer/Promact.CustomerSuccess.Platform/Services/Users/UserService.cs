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

        public async Task<Response> GetUserByUsernameAndEmailAsync(string username, string email)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.UserName == username && u.Email == email );

            if (user == null)
            {
                // User does not exist
                return new Response { message = "User not exist", User = null };
            }
            if (!user.active)
            {
                return new Response { message = "You are not verified user", User = null,IsSuccess=2 };
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

            // User exists
            return new Response { message = "You have successfully login", User = userDto ,IsSuccess=1};
        }

        public async Task<UserDto> GetDetailByEmailAsync(string email)
        {
            var user = (await _userRepository.GetListAsync(u => u.Email == email)).FirstOrDefault();
            return ObjectMapper.Map<User, UserDto>(user);
        }

        public async Task<List<UserDto>> GetUsersByRoleAsync(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                // Handle if roleName is null or empty
                return null;
            }

            // Fetch the role
            var role = await _roleRepository.FirstOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower());

            if (role == null)
            {
                // Handle if role not found
                return null;
            }

            // Fetch users with the specified role
            var userRoles = await _userRoleRepository.GetListAsync(ur => ur.RoleId == role.Id);
            var userIds = userRoles.Select(ur => ur.UserId);

            // Fetch users
            var users = await _userRepository.GetListAsync(u => userIds.Contains(u.Id));

            // Map users to UserDto
            var userDtos = users.Select(user => _mapper.Map<User, UserDto>(user)).ToList();

            return userDtos;
        }

        public override async Task<PagedResultDto<UserDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var users = await base.Repository.ToListAsync();

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
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

                userDtos.Add(userDto);
            }

            return new PagedResultDto<UserDto>(userDtos.Count, userDtos);
        }

        public async Task<bool> ToggleUserAccountStatus(Guid userId, bool isActive)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                user.active = isActive;
                await _userRepository.UpdateAsync(user,true);
                
                return true; // Account status updated successfully
            }

            return false; // User not found
        }


    }
    public class Response
    {
        public string message { get; set; }
        public UserDto User { get; set; }
        public int? IsSuccess { get; set; }  = 0;
    }
}
