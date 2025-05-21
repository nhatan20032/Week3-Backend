using EFCorePracticeAPI.Data;
using EFCorePracticeAPI.Models;
using EFCorePracticeAPI.Repository.Interface;

namespace EFCorePracticeAPI.Repository.Implement
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }
    }
}
