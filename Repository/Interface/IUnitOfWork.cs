namespace EFCorePracticeAPI.Repository.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        IUserRepository Users { get; }
        Task<int> CompleteAsync();
    }
}
