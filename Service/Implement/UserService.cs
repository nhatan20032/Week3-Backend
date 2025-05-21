using EFCorePracticeAPI.Models;
using EFCorePracticeAPI.Repository.Interface;
using EFCorePracticeAPI.Service.Interface;
using EFCorePracticeAPI.ViewModals;
using EFCorePracticeAPI.ViewModals.User;
using Microsoft.EntityFrameworkCore;

namespace EFCorePracticeAPI.Service.Implement
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<V_GetUser?> AddUser(V_User user)
        {
            var addUserResult = await _unitOfWork.Users.AddAsync(new User
            {
                Username = user.Username,
                Passwordhash = BCrypt.Net.BCrypt.HashPassword(user.Passwordhash),
                Fullname = string.IsNullOrEmpty(user.Fullname) ? $"User_{Guid.NewGuid()}" : user.Fullname,
            });

            await _unitOfWork.CompleteAsync();

            if (addUserResult == null) return null;

            return new V_GetUser
            {
                Id = addUserResult.Id,
                Username = addUserResult.Username,
                Fullname = addUserResult.Fullname,
                Passwordhash = addUserResult.Passwordhash,
                RoleId = addUserResult.Userroles!.Select(ur => ur.Role?.Id ?? 0).ToList(),
                RoleName = addUserResult.Userroles!.Select(ur => ur.Role?.Name ?? string.Empty).ToList()
            };
        }

        public async Task<V_GetUser?> UpdateUser(V_User user)
        {
            var existingUser = await _unitOfWork.Users.GetByIdAsync(user.Id);
            if (existingUser == null) return null;

            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                existingUser.Username = user.Username;
            }

            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                existingUser.Fullname = user.Fullname;
            }

            if (!string.IsNullOrWhiteSpace(user.Passwordhash))
            {
                existingUser.Passwordhash = BCrypt.Net.BCrypt.HashPassword(user.Passwordhash);
            }

            var updated = await _unitOfWork.Users.UpdateAsync(existingUser);
            await _unitOfWork.CompleteAsync();

            if (updated == null) return null;

            return new V_GetUser
            {
                Id = updated.Id,
                Username = updated.Username,
                Fullname = updated.Fullname,
                Passwordhash = updated.Passwordhash,
                RoleId = updated.Userroles?.Select(ur => ur.Role?.Id ?? 0).ToList() ?? [],
                RoleName = updated.Userroles?.Select(ur => ur.Role?.Name ?? string.Empty).ToList() ?? []
            };
        }

        public async Task<PagedResultDto<V_GetUser>> GetAllUser(int page = 1, int pageSize = 10, string search = "")
        {
            var pagedResult = await _unitOfWork.Users.GetAllAsync(
                pageNumber: page,
                pageSize: pageSize,
                filter: x => string.IsNullOrEmpty(search) ||
                        x.Username.ToLower().Contains(search.ToLower()) ||
                        x.Fullname!.ToLower().Contains(search.ToLower()),
                orderBy: q => q.OrderBy(x => x.Fullname),
                include: query => query
                        .Include(u => u.Userroles!)
                        .ThenInclude(ur => ur.Role!)
            );

            return new PagedResultDto<V_GetUser>
            {
                Data = pagedResult.Items.Select(user => new V_GetUser
                {
                    Id = user.Id,
                    Username = user.Username,
                    Fullname = user.Fullname,
                    Passwordhash = user.Passwordhash,
                    RoleId = user.Userroles!.Select(ur => ur.Role?.Id ?? 0).ToList(),
                    RoleName = user.Userroles!.Select(ur => ur.Role?.Name ?? string.Empty).ToList()
                }).ToList(),
                Meta = new PaginationMeta
                {
                    CurrentPage = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize,
                    TotalItems = pagedResult.TotalCount,
                    TotalPages = pagedResult.TotalPages
                }
            };
        }

        public async Task<V_GetUser?> GetUserById(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null) return null;

            return new V_GetUser
            {
                Id = user.Id,
                Username = user.Username,
                Fullname = user.Fullname,
                Passwordhash = user.Passwordhash,
                RoleName = user.Userroles!.Select(ur => ur.Role?.Name ?? string.Empty).ToList()
            };
        }

        public async Task<LoginResult<V_GetUser>?> Login(string username, string password)
        {
            var user = await _unitOfWork.Users.FindAsync(t => t.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Passwordhash))
                return null;

            return new LoginResult<V_GetUser>
            {
                Data = new V_GetUser
                {
                    Id = user.Id,
                    Username = user.Username,
                    Fullname = user.Fullname,
                    Passwordhash = user.Passwordhash,
                    RoleName = user.Userroles?.Select(ur => ur.Role?.Name ?? string.Empty).ToList() ?? [],
                    RoleId = user.Userroles?.Select(ur => ur.Role?.Id ?? 0).ToList() ?? []
                },
                TokenResult = new TokenResult
                {
                    Token = "",
                    Expiry = DateTime.UtcNow
                }
            };
        }


        public async Task<V_GetUser?> DeleteUser(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null) return null;

            var deletedUser = await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.CompleteAsync();

            if (deletedUser == null) return null;

            return new V_GetUser
            {
                Id = deletedUser.Id,
                Username = deletedUser.Username,
                Fullname = deletedUser.Fullname,
                Passwordhash = deletedUser.Passwordhash,
                RoleId = deletedUser.Userroles!.Select(ur => ur.Role?.Id ?? 0).ToList(),
                RoleName = deletedUser.Userroles!.Select(ur => ur.Role?.Name ?? string.Empty).ToList()
            };
        }
    }
}
