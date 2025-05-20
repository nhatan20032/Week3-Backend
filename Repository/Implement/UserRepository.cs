using EFCorePracticeAPI.Data;
using EFCorePracticeAPI.Models;
using EFCorePracticeAPI.Repository.Interface;

namespace EFCorePracticeAPI.Repository.Implement
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }
}
