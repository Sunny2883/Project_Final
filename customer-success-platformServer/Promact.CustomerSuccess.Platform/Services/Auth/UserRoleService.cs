using Microsoft.AspNetCore.Mvc;
using Promact.CustomerSuccess.Platform.Entities;
using Promact.CustomerSuccess.Platform.Services.Dtos.Auth;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Promact.CustomerSuccess.Platform.Services.Auth
{
    public class UserRoleService : CrudAppService<UserRole, UserRoleDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateUserRole, CreateUpdateUserRole>
    {
        private readonly IRepository<UserRole, Guid> _repository;
        public UserRoleService(IRepository<UserRole, Guid> repository) : base(repository)
        {

            this._repository = repository;
        }

        [HttpPost("assign-role")]
        public async Task<UserRoleDto> CreateOrUpdateUserRoleAsync(Guid userId, Guid roleId)
        {
            // Check if UserRole already exists for the given userId and roleId
            var existingUserRole = await _repository.FirstOrDefaultAsync(ur => ur.UserId == userId);

            if (existingUserRole != null)
            {
                // Update existing UserRole
                existingUserRole.UserId = userId;
                existingUserRole.RoleId = roleId;
                await Repository.UpdateAsync(existingUserRole);

                return ObjectMapper.Map<UserRole, UserRoleDto>(existingUserRole);
            }
            else
            {
                // Create new UserRole
                var newUserRole = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                };
                await Repository.InsertAsync(newUserRole, autoSave: true);

                return ObjectMapper.Map<UserRole, UserRoleDto>(newUserRole);
            }
        }

    }
}
