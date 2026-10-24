using Banyan.Entities.Models.secSchema;

namespace Banyan.Entities.viewModel;

public class UserSession
{
    public required string UserId { get; set; }
    public required string Token { get; set; }
    public RolePrivilege? RolePrivileges { get; set; }

}
