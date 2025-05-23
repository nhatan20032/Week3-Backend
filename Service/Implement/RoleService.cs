using EFCorePracticeAPI.Models;
using EFCorePracticeAPI.Repository.Interface;
using EFCorePracticeAPI.Service.Interface;
using EFCorePracticeAPI.ViewModals;
using EFCorePracticeAPI.ViewModals.Role;
using EFCorePracticeAPI.ViewModals.User;

namespace EFCorePracticeAPI.Service.Implement
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<V_Role>> GetAllRole(SearchDto searchDto)
        {
            var pagedResult = await _unitOfWork.Roles.GetAllAsync(
                pageNumber: searchDto.Page,
                pageSize: searchDto.PageSize,
                filter: x => string.IsNullOrEmpty(searchDto.Search) ||
                        x.Name.ToLower().Contains(searchDto.Search.ToLower()),
                orderBy: x => x.OrderBy(x => x.Name));

            return new PagedResultDto<V_Role>
            {
                Data = pagedResult.Items.Select(x => new V_Role
                {
                    Id = x.Id,
                    Name = x.Name,
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

        public async Task<V_Role?> GetRoleById(int id)
        {
            var result = await _unitOfWork.Roles.GetByIdAsync(id);

            return result == null
                ? throw new ApplicationException("Cannot find role. Try again!")
                : new V_Role
                {
                    Id = result.Id,
                    Name = result.Name,
                };
        }

        public async Task<V_Role?> AddRole(V_Role role)
        {
            var addResult = await _unitOfWork.Roles.AddAsync(new Role
            {
                Name = role.Name,
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
            };
        }

        public async Task<V_Role?> UpdateRole(V_Role role)
        {
            var existingItem = await _unitOfWork.Roles.GetByIdAsync(role.Id) ??
                               throw new ApplicationException("Cannot find role. Try again!");

            if (!string.IsNullOrWhiteSpace(role.Name))
            {
                existingItem.Name = role.Name;
            }

            var updated = await _unitOfWork.Roles.UpdateAsync(existingItem) ??
                          throw new ApplicationException("Failed to update role");

            await _unitOfWork.CompleteAsync();

            return new V_Role
            {
                Id = updated.Id,
                Name = updated.Name,
            };
        }

        public async Task<V_Role?> DeleteRole(int id)
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
            };
        }

        public async Task<V_GetUser> AddRoleForUser(V_RoleUser roleUser)
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
    }
}
