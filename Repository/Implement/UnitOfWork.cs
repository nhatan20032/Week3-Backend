using EFCorePracticeAPI.Data;
using EFCorePracticeAPI.Repository.Interface;
using System.Collections;

namespace EFCorePracticeAPI.Repository.Implement
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Hashtable _repositories = new();
        private IUserRepository? _userRepository;
        private IRefreshTokenRepository? _refreshTokenRepository;

        public UnitOfWork(AppDbContext context) => _context = context;

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(
                    repositoryType.MakeGenericType(typeof(T)), _context);

                if (repositoryInstance != null)
                {
                    _repositories.Add(type, repositoryInstance);
                }
            }
            return (IGenericRepository<T>)_repositories[type]!;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository ??= new RefreshTokenRepository(_context);
    }
}
