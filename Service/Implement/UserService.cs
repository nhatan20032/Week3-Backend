using EFCorePracticeAPI.Infrastructure;
using EFCorePracticeAPI.Models;
using EFCorePracticeAPI.Repository.Interface;
using EFCorePracticeAPI.Service.Interface;
using EFCorePracticeAPI.ViewModals;
using EFCorePracticeAPI.ViewModals.User;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EFCorePracticeAPI.Service.Implement
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenProvider _tokenProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUnitOfWork unitOfWork,
            TokenProvider tokenProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _tokenProvider = tokenProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<V_GetUser?> AddUser(V_User user)
        {
            var addResult = await _unitOfWork.Users.AddAsync(new User
            {
                Username = user.Username,
                Passwordhash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Fullname = string.IsNullOrEmpty(user.Fullname) ? $"User_{Guid.NewGuid()}" : user.Fullname,
            });

            await _unitOfWork.CompleteAsync();

            if (addResult == null)
            {
                throw new ApplicationException("Failed to create new user");
            }

            return new V_GetUser
            {
                Id = addResult.Id,
                Username = addResult.Username,
                Fullname = addResult.Fullname,
                Passwordhash = addResult.Passwordhash,
                RoleId = addResult.Userroles!.Select(ur => ur.Role?.Id ?? 0).ToList(),
                RoleName = addResult.Userroles!.Select(ur => ur.Role?.Name ?? string.Empty).ToList()
            };
        }

        public async Task<V_GetUser?> UpdateUser(V_User user)
        {
            var existingItem = await _unitOfWork.Users.GetByIdAsync(user.Id) ??
                               throw new ApplicationException("Cannot find user. Try again!");

            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                existingItem.Username = user.Username;
            }

            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                existingItem.Fullname = user.Fullname;
            }

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                existingItem.Passwordhash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            var updated = await _unitOfWork.Users.UpdateAsync(existingItem) ??
                          throw new ApplicationException("Failed to update user");

            await _unitOfWork.CompleteAsync();

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

        public async Task<PagedResultDto<V_GetUser>> GetAllUser(SearchDto searchDto)
        {
            var pagedResult = await _unitOfWork.Users.GetAllAsync(
                pageNumber: searchDto.Page,
                pageSize: searchDto.PageSize,
                filter: x => string.IsNullOrEmpty(searchDto.Search) ||
                        x.Username.ToLower().Contains(searchDto.Search.ToLower()) ||
                        x.Fullname!.ToLower().Contains(searchDto.Search.ToLower()),
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
            var result = await _unitOfWork.Users.GetByIdAsync(id);

            return result == null
                ? throw new ApplicationException("Cannot find user. Try again!")
                : new V_GetUser
                {
                    Id = result.Id,
                    Username = result.Username,
                    Fullname = result.Fullname,
                    Passwordhash = result.Passwordhash,
                    RoleName = result.Userroles!.Select(ur => ur.Role?.Name ?? string.Empty).ToList()
                };
        }

        public async Task<LoginResult<V_GetUser>?> Login(string username, string password)
        {
            var user = await _unitOfWork.Users.FindAsync(t => t.Username == username, include: t => t.Include(a => a.Userroles).ThenInclude(a => a.Role)!);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Passwordhash))
                throw new ApplicationException("Invalid account. Please check your username or password.");

            string token = _tokenProvider.Create(new V_GetUser
            {
                Id = user.Id,
                Username = user.Username,
                Fullname = user.Fullname,
                Passwordhash = user.Passwordhash,
                RoleName = user.Userroles?.Select(ur => ur.Role?.Name ?? string.Empty).ToList() ?? [],
                RoleId = user.Userroles?.Select(ur => ur.Role?.Id ?? 0).ToList() ?? []
            });

            if (string.IsNullOrEmpty(token))
            {
                throw new ApplicationException("Failed to generate token");
            }

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = _tokenProvider.GenerateRefreshToken(),
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);

            await _unitOfWork.CompleteAsync();

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
                    AccessToken = token,
                    RefreshToken = refreshToken.Token,
                }
            };
        }


        public async Task<V_GetUser?> DeleteUser(int id)
        {
            var item = await _unitOfWork.Users.GetByIdAsync(id) ??
                       throw new ApplicationException("Cannot find user. Try again!");

            var deletedItem = await _unitOfWork.Users.DeleteAsync(item) ??
                              throw new ApplicationException("Failed to delete user");

            await _unitOfWork.CompleteAsync();

            return new V_GetUser
            {
                Id = deletedItem.Id,
                Username = deletedItem.Username,
                Fullname = deletedItem.Fullname,
                Passwordhash = deletedItem.Passwordhash,
                RoleId = deletedItem.Userroles!.Select(ur => ur.Role?.Id ?? 0).ToList(),
                RoleName = deletedItem.Userroles!.Select(ur => ur.Role?.Name ?? string.Empty).ToList()
            };
        }

        public async Task<LoginResult<V_GetUser>?> Login(string refreshToken)
        {
            var token = await _unitOfWork.RefreshTokens.FindAsync(
                t => t.Token == refreshToken,
                include: query => query.Include(t => t.User));

            if (token == null || token.ExpiryDate < DateTime.UtcNow)
            {
                throw new ApplicationException("The refresh token has expired");
            }


            string accessToken = _tokenProvider.Create(new V_GetUser
            {
                Id = token.User.Id,
                Username = token.User.Username,
                Fullname = token.User.Fullname,
                Passwordhash = token.User.Passwordhash,
                RoleName = token.User.Userroles?.Select(ur => ur.Role?.Name ?? string.Empty).ToList() ?? [],
                RoleId = token.User.Userroles?.Select(ur => ur.Role?.Id ?? 0).ToList() ?? []
            });

            token.Token = _tokenProvider.GenerateRefreshToken();
            token.ExpiryDate = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.RefreshTokens.UpdateAsync(token);

            await _unitOfWork.CompleteAsync();

            return new LoginResult<V_GetUser>
            {
                Data = new V_GetUser
                {
                    Id = token.User.Id,
                    Username = token.User.Username,
                    Fullname = token.User.Fullname,
                    Passwordhash = token.User.Passwordhash,
                    RoleName = token.User.Userroles?.Select(ur => ur.Role?.Name ?? string.Empty).ToList() ?? [],
                    RoleId = token.User.Userroles?.Select(ur => ur.Role?.Id ?? 0).ToList() ?? []
                },
                TokenResult = new TokenResult
                {
                    AccessToken = accessToken,
                    RefreshToken = token.Token,
                }
            };
        }

        public async Task<bool> RevokeRefreshToken(int userId)
        {
            var currentUserId = int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out int idParse) ? idParse : 0;

            if (userId != currentUserId)
            {
                throw new ApplicationException("You are not authorized to revoke this token");
            }

            await _unitOfWork.RefreshTokens.DeleteAsync(t => t.UserId == userId);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
