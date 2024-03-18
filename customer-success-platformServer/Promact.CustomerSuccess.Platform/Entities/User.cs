using Promact.CustomerSuccess.Platform.Entities.Constants;
using Volo.Abp.Domain.Entities.Auditing;

namespace Promact.CustomerSuccess.Platform.Entities
{
    public class User:AuditedEntity<Guid>
    {
        public string Password { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }


    }
}
