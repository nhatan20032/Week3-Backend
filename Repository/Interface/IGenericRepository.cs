using EFCorePracticeAPI.ViewModals;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace EFCorePracticeAPI.Repository.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<PagedResult<T>> GetAllAsync(int pageNumber = 1,
            int pageSize = 10,
            Expression<Func<T, bool>> filter = null!,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null!,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null!);
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> expression);
        Task<T?> FindAsync(Expression<Func<T, bool>> expression);
    }
}
