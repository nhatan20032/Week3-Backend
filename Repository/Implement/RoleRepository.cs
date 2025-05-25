using EFCorePracticeAPI.Data;
using EFCorePracticeAPI.Models;
using EFCorePracticeAPI.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace EFCorePracticeAPI.Repository.Implement
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        private new readonly AppDbContext _context;

        public RoleRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Userrole>> CreateUserRole(int userId, List<int> roleIds)
        {
            if (roleIds == null || !roleIds.Any())
            {
                var roleId = await _context.Set<Role>().FirstOrDefaultAsync(r => r.IsDefault == true);
                if (roleId != null)
                {
                    roleIds = new List<int> { roleId.Id };
                }
                else
                {
                    throw new ApplicationException("No roles provided and no default role found.");
                }
            }

            var userRoles = roleIds.Select(roleId => new Userrole
            {
                Userid = userId,
                Roleid = roleId
            }).ToList();

            await _context.Set<Userrole>().AddRangeAsync(userRoles);

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

            return newUserRoles;
        }
    }
}
