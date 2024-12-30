using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Data;
using MvcLaptop.Models;

namespace MvcLaptop.Services;
public class LaptopService : ILaptopService
{
    private readonly MvcLaptopContext _context;
    private readonly IMapper _mapper;
    
    public LaptopService(MvcLaptopContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Laptop> Create(LaptopRequest request)
    {
        var laptop = _mapper.Map<Laptop>(request);
        _context.Add(laptop);
        await _context.SaveChangesAsync();
        return laptop;
    }

    public async Task<bool> Delete(int id)
    {
        var laptop = await _context.Laptop.FindAsync(id);
        if (laptop != null)
        {
            _context.Laptop.Remove(laptop);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<LaptopViewModel> GetLaptopById(int id)
    {
        var laptop = await _context.Laptop.FindAsync(id);
        return _mapper.Map<LaptopViewModel>(laptop);
    }

    public async Task<IEnumerable<LaptopViewModel>> GetLaptops(string? searchString = null)
    {
         // Lọc danh sách laptop theo từ khóa (nếu có)
        var laptops = await _context.Laptop
        .Where(l => string.IsNullOrEmpty(searchString) || 
                    (l.Title != null && l.Title.ToUpper().Contains(searchString.ToUpper())))
        .ToListAsync();
        return _mapper.Map<IEnumerable<LaptopViewModel>>(laptops);
    }

    public bool LaptopExists(int id)
    {
        return _context.Laptop.Any(e => e.Id == id);
    }

    public async Task<bool> Update(int id, LaptopViewModel laptop)
    {
        _context.Update(_mapper.Map<Laptop>(laptop));
        await _context.SaveChangesAsync();
        return true;
    }
}