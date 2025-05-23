using EFCorePracticeAPI.Data;
using EFCorePracticeAPI.Models;
using EFCorePracticeAPI.Repository.Interface;

namespace EFCorePracticeAPI.Repository.Implement
{
    public class RoleRepository(AppDbContext context) : GenericRepository<Role>(context), IRoleRepository
    {
        public async Task<List<Userrole>> CreateUserRole(int userId, List<int> roleIds)
        {
            var userRoles = roleIds.Select(roleId => new Userrole
            {
                Userid = userId,
                Roleid = roleId
            }).ToList();

            await _context.Set<Userrole>().AddRangeAsync(userRoles);

            await _context.SaveChangesAsync();

            return userRoles;
        }
    }
}
