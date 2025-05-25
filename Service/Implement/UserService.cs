using EFCorePracticeAPI.CustomException;
using EFCorePracticeAPI.Infrastructure;
using EFCorePracticeAPI.Models;
using EFCorePracticeAPI.Repository.Interface;
using EFCorePracticeAPI.Service.Interface;
using EFCorePracticeAPI.ViewModals;
using EFCorePracticeAPI.ViewModals.User;
using Microsoft.EntityFrameworkCore;
using Serilog;
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

        public async Task<V_GetUser?> AddUser(V_CreateUser user)
        {
            try
            {
                var addResult = await _unitOfWork.Users.AddAsync(new User
                {
                    Username = user.Username,
                    Passwordhash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    Email = user.Email!.Trim(),
                    Fullname = string.IsNullOrWhiteSpace(user.Fullname) ? $"User_{Guid.NewGuid()}" : user.Fullname.Trim(),
                });

                await _unitOfWork.CompleteAsync();

                if (addResult == null)
                {
                    throw new ApplicationException("Failed to create new user");
                }

                await _unitOfWork.Roles.CreateUserRole(addResult.Id, user.RoleIds);
                await _unitOfWork.CompleteAsync();

                var reloaded = await _unitOfWork.Users.FindAsync(
                    u => u.Id == addResult.Id,
                    include: q => q.Include(u => u.Userroles).ThenInclude(ur => ur.Role)!);

                return new V_GetUser
                {
                    Id = reloaded!.Id,
                    Username = reloaded.Username,
                    Fullname = reloaded.Fullname,
                    Email = reloaded.Email,
                    Passwordhash = reloaded.Passwordhash,
                    RoleId = reloaded.Userroles!.Select(ur => ur.Role?.Id ?? 0).ToList(),
                    RoleName = reloaded.Userroles!.Select(ur => ur.Role?.Name ?? string.Empty).ToList()
                };
            }
            catch (ApplicationException ex)
            {
                Log.Warning(ex, "Application error while adding user");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled error while adding user");
                throw;
            }
        }

        public async Task<V_GetUser?> UpdateUser(V_UpdateUser user)
        {
            try
            {
                var existingItem = await _unitOfWork.Users.GetByIdAsync(user.Id)
                    ?? throw new ApplicationException("Cannot find user. Try again!");

                if (!string.IsNullOrWhiteSpace(user.Fullname))
                    existingItem.Fullname = user.Fullname.Trim();

                if (!string.IsNullOrWhiteSpace(user.Email))
                    existingItem.Email = user.Email;

                if (!string.IsNullOrWhiteSpace(user.Password))
                    existingItem.Passwordhash = BCrypt.Net.BCrypt.HashPassword(user.Password);

                if (user.RoleIds != null && user.RoleIds.Count > 0)
                    await _unitOfWork.Roles.UpdateUserRole(existingItem.Id, user.RoleIds);

                var updated = await _unitOfWork.Users.UpdateAsync(existingItem)
                    ?? throw new ApplicationException("Failed to update user");

                await _unitOfWork.CompleteAsync();

                var reloaded = await _unitOfWork.Users.FindAsync(
                    u => u.Id == updated.Id,
                    include: q => q.Include(u => u.Userroles).ThenInclude(ur => ur.Role)!);

                return new V_GetUser
                {
                    Id = reloaded!.Id,
                    Username = reloaded.Username,
                    Fullname = reloaded.Fullname,
                    Email = reloaded.Email,
                    Passwordhash = reloaded.Passwordhash,
                    RoleId = reloaded.Userroles?.Select(ur => ur.Roleid ?? 0).ToList() ?? [],
                    RoleName = reloaded.Userroles?.Select(ur => ur.Role?.Name ?? string.Empty).ToList() ?? []
                };
            }
            catch (ApplicationException ex)
            {
                Log.Warning(ex, "Application error while updating user");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled error while updating user");
                throw;
            }
        }

        public async Task<PagedResultDto<V_GetUser>> GetAllUser(SearchDto searchDto)
        {
            try
            {
                var pagedResult = await _unitOfWork.Users.GetAllAsync(
                    searchDto.Page,
                    searchDto.PageSize,
                    x => string.IsNullOrEmpty(searchDto.Search) ||
                        x.Username.ToLower().Contains(searchDto.Search.ToLower()) ||
                        x.Fullname!.ToLower().Contains(searchDto.Search.ToLower()) ||
                        x.Email!.ToLower().Contains(searchDto.Search.ToLower()),
                    q => q.OrderBy(x => x.Fullname),
                    query => query.Include(u => u.Userroles!).ThenInclude(ur => ur.Role!)
                );

                return new PagedResultDto<V_GetUser>
                {
                    Data = pagedResult.Items.Select(user => new V_GetUser
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Fullname = user.Fullname,
                        Email = user.Email,
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
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all users");
                throw;
            }
        }


        public async Task<V_GetUser?> GetUserById(int id)
        {
            try
            {
                var result = await _unitOfWork.Users.GetByIdAsync(id)
                    ?? throw new ApplicationException("Cannot find user. Try again!");

                return new V_GetUser
                {
                    Id = result.Id,
                    Username = result.Username,
                    Email = result.Email,
                    Fullname = result.Fullname,
                    Passwordhash = result.Passwordhash,
                    RoleName = result.Userroles!.Select(ur => ur.Role?.Name ?? string.Empty).ToList()
                };
            }
            catch (ApplicationException ex)
            {
                Log.Warning(ex, "Application error getting user by id");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled error getting user by id");
                throw;
            }
        }

        public async Task<LoginResult<V_GetUser>?> Login(string username, string password)
        {
            try
            {
                var user = await _unitOfWork.Users.FindAsync(
                    t => t.Username == username,
                    t => t.Include(a => a.Userroles).ThenInclude(a => a.Role)!);

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Passwordhash))
                    throw new ApplicationException("Invalid account. Please check your username or password.");

                string token = _tokenProvider.Create(new V_GetUser
                {
                    Id = user.Id,
                    Username = user.Username,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    Passwordhash = user.Passwordhash,
                    RoleName = user.Userroles?.Select(ur => ur.Role?.Name ?? string.Empty).ToList() ?? [],
                    RoleId = user.Userroles?.Select(ur => ur.Role?.Id ?? 0).ToList() ?? []
                });

                if (string.IsNullOrEmpty(token))
                    throw new ApplicationException("Failed to generate token");

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
                        Email = user.Email,
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
            catch (ApplicationException ex)
            {
                Log.Warning(ex, "Application error during login");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled error during login");
                throw;
            }
        }

        public async Task<LoginResult<V_GetUser>?> Login(string refreshToken)
        {
            try
            {
                var token = await _unitOfWork.RefreshTokens.FindAsync(
                    t => t.Token == refreshToken,
                    include: query => query.Include(t => t.User));

                if (token == null || token.ExpiryDate < DateTime.UtcNow)
                    throw new ApplicationException("The refresh token has expired");

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
            catch (ApplicationException ex)
            {
                Log.Warning(ex, "Application error with refresh token login");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled error with refresh token login");
                throw;
            }
        }

        public async Task<V_GetUser?> DeleteUser(int id)
        {
            try
            {
                var item = await _unitOfWork.Users.GetByIdAsync(id)
                    ?? throw new ApplicationException("Cannot find user. Try again!");

                var deletedItem = await _unitOfWork.Users.DeleteAsync(item)
                    ?? throw new ApplicationException("Failed to delete user");

                await _unitOfWork.CompleteAsync();

                return new V_GetUser
                {
                    Id = deletedItem.Id,
                    Username = deletedItem.Username,
                    Fullname = deletedItem.Fullname,
                    Email = deletedItem.Email,
                    Passwordhash = deletedItem.Passwordhash,
                    RoleId = deletedItem.Userroles!.Select(ur => ur.Role?.Id ?? 0).ToList(),
                    RoleName = deletedItem.Userroles!.Select(ur => ur.Role?.Name ?? string.Empty).ToList()
                };
            }
            catch (ApplicationException ex)
            {
                Log.Warning(ex, "Application error deleting user");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled error deleting user");
                throw;
            }
        }

        public async Task<bool> RevokeRefreshToken(int userId)
        {
            try
            {
                var currentUserId = int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out int idParse) ? idParse : 0;

                if (userId != currentUserId)
                    throw new ForbiddenException("You are not authorized to revoke this token");

                await _unitOfWork.RefreshTokens.DeleteAsync(t => t.UserId == userId);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (ForbiddenException ex)
            {
                Log.Warning(ex, "Forbidden action during refresh token revocation");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled error revoking refresh token");
                throw;
            }
        }
    }
}
