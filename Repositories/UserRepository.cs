using MvcLaptop.Models;

namespace MvcLaptop.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<Pagination<User>> GetUsersPagedAsync(int pageNumber, int pageSize, string name);
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> GetUserByNameAsync(string id);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string id);
        Task<bool> UserExistsAsync(string id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly UnitOfWork _unitOfWork;
        public UserRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _unitOfWork._userRepository.GetAllAsync();
        }

        public async Task<Pagination<User>> GetUsersPagedAsync(int pageNumber, int pageSize, string name)
        {
            // Calculate the number of items to skip based on the page number and page size
            int skipCount = (pageNumber - 1) * pageSize;

            // Get the total count of records that match the filter (for pagination calculation)
            int totalRecords = await _unitOfWork._userRepository.CountAsync(
                predicate: c => c.Email!.Contains(name)
            );

            // Apply the filtering, pagination, and sorting
            var users = await _unitOfWork._userRepository.GetManyAsync(
                predicate: c => c.Email!.Contains(name),  // Filter by email
                queryOperation: query => query.Skip(skipCount).Take(pageSize) // Apply paging and sorting
            );

            // Return the Pagination object containing the results and pagination information
            return new Pagination<User>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Items = users.ToList() // The paged list of users
            };
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _unitOfWork._userRepository.GetByIdAsync(id);
        }

        public async Task CreateUserAsync(User user)
        {
            _unitOfWork._userRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _unitOfWork._userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _unitOfWork._userRepository.GetByIdAsync(id);
            if (user != null)
            {
                _unitOfWork._userRepository.Remove(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> UserExistsAsync(string id)
        {
            return await _unitOfWork._userRepository.ExistsAsync(id);
        }

        public Task<User?> GetUserByNameAsync(string id)
        {
            return _unitOfWork._userRepository
                .FindAsync(c => c.Email == id);
        }
    }
}