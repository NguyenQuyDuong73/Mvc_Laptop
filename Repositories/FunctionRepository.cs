using MvcLaptop.Models;

namespace MvcLaptop.Repositories
{
    public interface IFunctionRepository
    {
        Task<IEnumerable<Function>> GetAllFunctionsAsync();
        Task<Pagination<Function>> GetFunctionsPagedAsync(int pageNumber, int pageSize, string name);
        Task<Function?> GetFunctionByIdAsync(string id);
        Task CreateFunctionAsync(Function function);
        Task UpdateFunctionAsync(Function function);
        Task DeleteFunctionAsync(string id);
        Task<bool> FunctionExistsAsync(string id);
    }

    public class FunctionRepository : IFunctionRepository
    {
        private readonly UnitOfWork _unitOfWork;
        public FunctionRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Function>> GetAllFunctionsAsync()
        {
            return await _unitOfWork._functionRepository.GetAllAsync();
        }

        public async Task<Pagination<Function>> GetFunctionsPagedAsync(int pageNumber, int pageSize, string name)
        {
            // Calculate the number of items to skip based on the page number and page size
            int skipCount = (pageNumber - 1) * pageSize;

            // Get the total count of records that match the filter (for pagination calculation)
            int totalRecords = await _unitOfWork._functionRepository.CountAsync();

            // Apply the filtering, pagination, and sorting
            var functions = await _unitOfWork._functionRepository.GetManyAsync(
                queryOperation: query => query.Skip(skipCount).Take(pageSize) // Apply paging and sorting
            );

            // Return the Pagination object containing the results and pagination information
            return new Pagination<Function>
            {
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Items = functions.ToList() // The paged list of functions
            };
        }


        public async Task<Function?> GetFunctionByIdAsync(string id)
        {
            return await _unitOfWork._functionRepository.GetByIdAsync(id);
        }

        public async Task CreateFunctionAsync(Function function)
        {
            _unitOfWork._functionRepository.Add(function);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateFunctionAsync(Function function)
        {
            _unitOfWork._functionRepository.Update(function);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteFunctionAsync(string id)
        {
            var function = await _unitOfWork._functionRepository.GetByIdAsync(id);
            if (function != null)
            {
                _unitOfWork._functionRepository.Remove(function);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> FunctionExistsAsync(string id)
        {
            return await _unitOfWork._functionRepository.ExistsAsync(id);
        }
    }
}