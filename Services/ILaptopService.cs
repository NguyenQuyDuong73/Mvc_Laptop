using MvcLaptop.Models;

namespace MvcLaptop.Services;

public interface ILaptopService
{
    Task<IEnumerable<LaptopViewModel>> GetLaptops(string? searchString = null);
    Task<LaptopViewModel> GetLaptopById(int id);
    Task<Laptop> Create(LaptopRequest request);
    Task<bool> Update(int id, LaptopViewModel laptop);
    Task<bool> Delete(int id);
    bool LaptopExists(int id);
}
