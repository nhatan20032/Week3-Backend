using EFCorePracticeAPI.ViewModals;
using EFCorePracticeAPI.ViewModals.User;

namespace EFCorePracticeAPI.Service.Interface
{
    public interface IUserService
    {
        Task<PagedResultDto<V_GetUser>> GetAllUser(int page = 1, int pageSize = 10, string search = "");
        Task<V_GetUser?> GetUserById(int id);
        Task<V_GetUser?> AddUser(V_User user);
        Task<V_GetUser?> UpdateUser(V_User user);
        Task<LoginResult<V_GetUser>?> Login(string username, string password);
        Task<LoginResult<V_GetUser>?> LoginWithRefreshToken(string refreshToken);
        Task<V_GetUser?> DeleteUser(int id);
    }
}
