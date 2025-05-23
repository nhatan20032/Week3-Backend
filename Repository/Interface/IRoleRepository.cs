using EFCorePracticeAPI.Models;

namespace EFCorePracticeAPI.Repository.Interface
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<List<Userrole>> CreateUserRole(int userId, List<int> roleId);
    }
}
