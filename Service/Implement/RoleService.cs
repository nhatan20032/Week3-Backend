using EFCorePracticeAPI.Models;
using EFCorePracticeAPI.Repository.Interface;
using EFCorePracticeAPI.Service.Interface;
using EFCorePracticeAPI.ViewModals;
using EFCorePracticeAPI.ViewModals.Role;
using EFCorePracticeAPI.ViewModals.User;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EFCorePracticeAPI.Service.Implement
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public RoleService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<PagedResultDto<V_Role>> GetAllRole(SearchDto searchDto)
        {
            try
            {
                var pagedResult = await _unitOfWork.Roles.GetAllAsync(
                pageNumber: searchDto.Page,
                pageSize: searchDto.PageSize,
                filter: x => string.IsNullOrEmpty(searchDto.Search!.Trim()) ||
                        x.Name.ToLower().Contains(searchDto.Search.Trim().ToLower()),
                orderBy: x => x.OrderBy(x => x.Name));

                return new PagedResultDto<V_Role>
                {
                    Data = pagedResult.Items.Select(x => new V_Role
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsDefault = x.IsDefault
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
            catch (ApplicationException ex)
            {
                Log.Error("User '{User}' failed to get all roles. Application error: {Error}", _currentUser.Username, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error($"Message: {ex}");
                throw;
            }
        }

        public async Task<V_Role?> GetRoleById(int id)
        {
            try
            {
                var result = await _unitOfWork.Roles.GetByIdAsync(id);

                return result == null
                    ? throw new ApplicationException("Cannot find role. Try again!")
                    : new V_Role
                    {
                        Id = result.Id,
                        Name = result.Name,
                        IsDefault = result.IsDefault
                    };
            }
            catch (ApplicationException ex)
            {
                Log.Error("User '{User}' failed to get role by ID {Id}. Application error: {Error}", _currentUser.Username, id, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error($"Message: {ex}");
                throw;
            }
        }

        public async Task<V_Role?> AddRole(V_Role role)
        {
            try
            {
                var addResult = await _unitOfWork.Roles.AddAsync(new Role
                {
                    Name = role.Name.Trim().ToUpper(),
                    IsDefault = role.IsDefault
                });

                await _unitOfWork.CompleteAsync();

                if (addResult == null)
                {
                    throw new ApplicationException("Failed to create new role");
                }

                return new V_Role
                {
                    Id = addResult.Id,
                    Name = addResult.Name,
                    IsDefault = addResult.IsDefault
                };
            }
            catch (ApplicationException ex)
            {
                Log.Error("User '{User}' failed to add role. Application error: {Error}", _currentUser.Username, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error($"Message: {ex}");
                throw;
            }
        }

        public async Task<V_Role?> UpdateRole(V_Role role)
        {
            try
            {
                var existingItem = await _unitOfWork.Roles.GetByIdAsync(role.Id) ??
                               throw new ApplicationException("Cannot find role. Try again!");

                if (!string.IsNullOrWhiteSpace(role.Name))
                {
                    existingItem.Name = role.Name.Trim().ToUpper();
                }

                var updated = await _unitOfWork.Roles.UpdateAsync(existingItem) ??
                              throw new ApplicationException("Failed to update role");

                await _unitOfWork.CompleteAsync();

                return new V_Role
                {
                    Id = updated.Id,
                    Name = updated.Name,
                    IsDefault = updated.IsDefault
                };
            }
            catch (ApplicationException ex)
            {
                Log.Error("User '{User}' failed to update role ID {Id}. Application error: {Error}", _currentUser.Username, role.Id, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error($"Message: {ex}");
                throw;
            }
        }

        public async Task<V_Role?> DeleteRole(int id)
        {
            try
            {
                var item = await _unitOfWork.Roles.GetByIdAsync(id) ??
                           throw new ApplicationException("Cannot find Role. Try again!");

                var deletedItem = await _unitOfWork.Roles.DeleteAsync(item) ??
                                  throw new ApplicationException("Failed to delete Role");

                await _unitOfWork.CompleteAsync();

                return new V_Role
                {
                    Id = deletedItem.Id,
                    Name = deletedItem.Name,
                    IsDefault = deletedItem.IsDefault
                };
            }
            catch (DbUpdateException)
            {
                Log.Error("User '{User}' attempted to delete role with ID {Id}, but it is in use.", _currentUser.Username, id);
                throw new ApplicationException("The role is currently in use and cannot be deleted.");
            }
            catch (ApplicationException)
            {
                Log.Error("User '{User}' attempted to delete role with ID {Id}, but it does not exist.", _currentUser.Username, id);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error("User '{User}' encountered unexpected error deleting role ID {Id}. Error: {Error}",
                          _currentUser.Username, id, ex.Message);
                throw;
            }
        }

        public async Task<V_GetUser> AddRoleForUser(V_RoleUser roleUser)
        {
            try
            {
                var result = await _unitOfWork.Roles.CreateUserRole(roleUser.UserId, roleUser.RoleIds);
                var user = await _unitOfWork.Users.GetByIdAsync(roleUser.UserId);

                if (result == null)
                {
                    throw new ApplicationException("Cannot add role for user, please try again");
                }

                if (user == null)
                {
                    throw new ApplicationException("Cannot find user. Try again!");
                }

                return new V_GetUser
                {
                    Id = user.Id,
                    Username = user.Username,
                    Passwordhash = user.Passwordhash,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    RoleId = result.Select(r => r.Role!.Id).ToList(),
                    RoleName = result.Select(r => r.Role!.Name).ToList()
                };
            }
            catch (ApplicationException ex)
            {
                Log.Error("User '{User}' failed to assign roles to user ID {UserId}. Application error: {Error}", _currentUser.Username, roleUser.UserId, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error("User '{User}' failed to assign roles to user ID {UserId}. Error: {Error}",
                          _currentUser.Username, roleUser.UserId, ex.Message);
                throw;
            }
        }
    }
}
