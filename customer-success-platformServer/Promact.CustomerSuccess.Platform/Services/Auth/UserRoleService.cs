using Promact.CustomerSuccess.Platform.Entities;
using Promact.CustomerSuccess.Platform.Services.Dtos.Auth;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Promact.CustomerSuccess.Platform.Services.Auth
{
    public class UserRoleService : CrudAppService<UserRole, UserRoleDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateUserRole, CreateUpdateUserRole>
    {
        public UserRoleService(IRepository<UserRole, Guid> repository) : base(repository)
        {

        }
    }
}
