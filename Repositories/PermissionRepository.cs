using Mvclaptop.Models;
using MvcLaptop.Models;

namespace Mvclaptop.Repositories
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();
        Task<IEnumerable<CommandInFunctionViewModel>> GetAllCommandInFunctionsAsync();
        Task<Pagination<Permission>> GetPermissionsPagedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(string roleId);
        Task<Permission?> GetPermissionByIdAsync(string id);
        Task CreatePermissionAsync(Permission permission);
        Task UpdatePermissionAsync(Permission permission);
        Task DeletePermissionAsync(string id);
        Task<bool> PermissionExistsAsync(string id);
        Task UpdateRolePermissionsAsync(string roleId, IEnumerable<string> newPermissions);
        Task<List<string>> GetPermissionsByRolesAsync(List<string> roleNames);
    }

    public class PermissionRepository : IPermissionRepository
    {
        private readonly UnitOfWork _unitOfWork;
        public PermissionRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
        {
            return await _unitOfWork._permissionRepository.GetAllAsync();
        }

        public async Task<IEnumerable<CommandInFunctionViewModel>> GetAllCommandInFunctionsAsync()
        {
            var functions = await _unitOfWork._functionRepository.GetAllAsync();
            var commands = await _unitOfWork._commandRepository.GetAllAsync();

            var commandsInFunctions = new List<CommandInFunctionViewModel>();
            foreach (var function in functions)
            {
                foreach (var command in commands)
                {
                    commandsInFunctions.Add(new CommandInFunctionViewModel
                    {
                        Command = command,
                        Function = function
                    });
                }
            }
            return commandsInFunctions;
        }
        public async Task<Pagination<Permission>> GetPermissionsPagedAsync(int pageNumber, int pageSize)
        {
            // Calculate the number of items to skip based on the page number and page size
            int skipCount = (pageNumber - 1) * pageSize;

            // Get the total count of records that match the filter (for pagination calculation)
            int totalRecords = await _unitOfWork._permissionRepository.CountAsync();

            // Apply the filtering, pagination, and sorting
            var permissions = await _unitOfWork._permissionRepository.GetManyAsync(
                // Filter by name
                queryOperation: query => query.Skip(skipCount).Take(pageSize) // Apply paging and sorting
            );

            // Return the Pagination object containing the results and pagination information
            return new Pagination<Permission>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Items = permissions.ToList() // The paged list of permissions
            };
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(string roleId)
        {
            // Assuming there's a relation between roles and permissions in the database,
            // and RoleId is a foreign key in the Permission entity.

            // Query for permissions associated with the given roleId
            return await _unitOfWork._permissionRepository.GetManyAsync(
                predicate: p => p.RoleId == roleId // Assuming Permission entity has RoleId property
            );
        }

        public async Task<Permission?> GetPermissionByIdAsync(string id)
        {
            return await _unitOfWork._permissionRepository.GetByIdAsync(id);
        }

        public async Task CreatePermissionAsync(Permission permission)
        {
            _unitOfWork._permissionRepository.Add(permission);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePermissionAsync(Permission permission)
        {
            _unitOfWork._permissionRepository.Update(permission);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePermissionAsync(string id)
        {
            var permission = await _unitOfWork._permissionRepository.GetByIdAsync(id);
            if (permission != null)
            {
                _unitOfWork._permissionRepository.Remove(permission);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> PermissionExistsAsync(string id)
        {
            return await _unitOfWork._permissionRepository.ExistsAsync(id);
        }

        public async Task UpdateRolePermissionsAsync(string roleId, IEnumerable<string> selectedPermissions)
        {
            // Get existing permissions for the role
            var existingPermissions = await _unitOfWork._permissionRepository.GetManyAsync(p => p.RoleId == roleId);

            // Convert existing permissions to a list of identifiers for easier checking
            var existingPermissionIds = existingPermissions
                .Select(p => $"{p.FunctionId}-{p.CommandId}")
                .ToList();

            // Remove permissions that were not selected
            foreach (var existing in existingPermissions)
            {
                var identifier = $"{existing.FunctionId}-{existing.CommandId}";
                if (!selectedPermissions.Contains(identifier))
                {
                    _unitOfWork._permissionRepository.Remove(existing);
                }
            }

            // Add new permissions that were selected but not previously assigned
            foreach (var selected in selectedPermissions)
            {
                var parts = selected.Split('-'); // Assuming the format is FunctionId_CommandId
                if (parts.Length == 2)
                {
                    var functionId = parts[0];
                    var commandId = parts[1];

                    var existingPermission = existingPermissions
                        .FirstOrDefault(p => p.FunctionId == functionId && p.CommandId == commandId);

                    // Only add new permission if it doesn't already exist
                    if (existingPermission == null)
                    {
                        var newPermission = new Permission(functionId, roleId, commandId);
                        _unitOfWork._permissionRepository.Add(newPermission);
                    }
                }
            }

            // Save changes to the database
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<string>> GetPermissionsByRolesAsync(List<string> roleNames)
        {
            var permissions = await _unitOfWork._permissionRepository.GetAllAsync(x => x.Function!, x => x.Command!, x => x.Role!);
            return permissions
                .Where(p => roleNames.Contains(p.Role!.Name!))
                .Select(p => $"{p.FunctionId}_{p.CommandId}")
                .ToList();
        }
    }
}
