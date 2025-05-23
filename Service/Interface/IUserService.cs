using EFCorePracticeAPI.ViewModals;
using EFCorePracticeAPI.ViewModals.User;

namespace EFCorePracticeAPI.Service.Interface
{
    public interface IUserService
    {
        Task<PagedResultDto<V_GetUser>> GetAllUser(SearchDto searchDto);
        Task<V_GetUser?> GetUserById(int id);
        Task<V_GetUser?> AddUser(V_CreateUser user);
        Task<V_GetUser?> UpdateUser(V_UpdateUser user);
        Task<LoginResult<V_GetUser>?> Login(string username, string password);
        Task<LoginResult<V_GetUser>?> Login(string refreshToken);
        Task<bool> RevokeRefreshToken(int userId);
        Task<V_GetUser?> DeleteUser(int id);
    }
}
