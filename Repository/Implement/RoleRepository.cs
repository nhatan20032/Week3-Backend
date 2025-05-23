using EFCorePracticeAPI.Data;
using EFCorePracticeAPI.Models;
using EFCorePracticeAPI.Repository.Interface;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Userrole>> UpdateUserRole(int userId, List<int> roleIds)
        {
            var existingUserRoles = await _context.Set<Userrole>()
                .Where(ur => ur.Userid == userId)
                .ToListAsync();

            _context.Set<Userrole>().RemoveRange(existingUserRoles);

            var newUserRoles = roleIds.Select(roleId => new Userrole
            {
                Userid = userId,
                Roleid = roleId
            }).ToList();

            await _context.Set<Userrole>().AddRangeAsync(newUserRoles);

            await _context.SaveChangesAsync();

            return newUserRoles;
        }

    }
}
