using EFCorePracticeAPI.ViewModals;
using EFCorePracticeAPI.ViewModals.Role;
using EFCorePracticeAPI.ViewModals.User;

namespace EFCorePracticeAPI.Service.Interface
{
    public interface IRoleService
    {
        Task<PagedResultDto<V_Role>> GetAllRole(SearchDto searchDto);

        Task<V_Role?> GetRoleById(int id);

        Task<V_Role?> AddRole(V_Role role);

        Task<V_Role?> UpdateRole(V_Role role);

        Task<V_Role?> DeleteRole(int id);

        Task<V_GetUser> AddRoleForUser(V_RoleUser roleUser);
    }
}
