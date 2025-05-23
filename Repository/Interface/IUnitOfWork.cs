namespace EFCorePracticeAPI.Repository.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IRoleRepository Roles { get; }
        Task<int> CompleteAsync();
    }
}
