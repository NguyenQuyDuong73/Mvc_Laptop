using Mvclaptop.Models;
using MvcLaptop.Models;

namespace Mvclaptop.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Pagination<Role>> GetRolesPagedAsync(int pageNumber, int pageSize, string name);
        Task<Role?> GetRoleByIdAsync(string id);
        Task CreateRoleAsync(Role role);
        Task UpdateRoleAsync(Role role);
        Task DeleteRoleAsync(string id);
        Task<bool> RoleExistsAsync(string id);
    }

    public class RoleRepository : IRoleRepository
    {
        private readonly UnitOfWork _unitOfWork;
        public RoleRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _unitOfWork._roleRepository.GetAllAsync();
        }

        public async Task<Pagination<Role>> GetRolesPagedAsync(int pageNumber, int pageSize, string name)
        {
            // Calculate the number of items to skip based on the page number and page size
            int skipCount = (pageNumber - 1) * pageSize;

            // Get the total count of records that match the filter (for pagination calculation)
            int totalRecords = await _unitOfWork._roleRepository.CountAsync(
                predicate: c => c.Name!.Contains(name)
            );

            // Apply the filtering, pagination, and sorting
            var roles = await _unitOfWork._roleRepository.GetManyAsync(
                predicate: c => c.Name!.Contains(name),  // Filter by name
                queryOperation: query => query.Skip(skipCount).Take(pageSize) // Apply paging and sorting
            );

            // Return the Pagination object containing the results and pagination information
            return new Pagination<Role>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Items = roles.ToList() // The paged list of roles
            };
        }


        public async Task<Role?> GetRoleByIdAsync(string id)
        {
            return await _unitOfWork._roleRepository.GetByIdAsync(id);
        }

        public async Task CreateRoleAsync(Role role)
        {
            _unitOfWork._roleRepository.Add(role);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateRoleAsync(Role role)
        {
            _unitOfWork._roleRepository.Update(role);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(string id)
        {
            var role = await _unitOfWork._roleRepository.GetByIdAsync(id);
            if (role != null)
            {
                _unitOfWork._roleRepository.Remove(role);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> RoleExistsAsync(string id)
        {
            return await _unitOfWork._roleRepository.ExistsAsync(id);
        }
    }
}
