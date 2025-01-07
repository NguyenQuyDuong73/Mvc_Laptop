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

    public async Task<Product> Create(LaptopRequest request, IFormFile mainImage)
    {
        var laptop = _mapper.Map<Product>(request);

        // Xử lý ảnh chính
        if (mainImage != null && mainImage.Length > 0)
        {
            var fileName = Path.GetFileNameWithoutExtension(mainImage.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(mainImage.FileName);
            var filePath = Path.Combine("wwwroot/images", fileName);

            // Lưu file lên server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await mainImage.CopyToAsync(stream);
            }

            // Gán ảnh chính
            laptop.ProductImages = new List<ProductImage>
        {
            new ProductImage
            {
                ImageUrl = $"/images/{fileName}",
                IsMainImage = true
            }
        };
        }

        _context.Add(laptop);
        await _context.SaveChangesAsync();
        return laptop;
    }

    public async Task<bool> Delete(int id)
    {
        var laptop = await _context.Product.FindAsync(id);
        if (laptop != null)
        {
            _context.Product.Remove(laptop);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await _context.Category!.OrderBy(c => c.Name_Category).ToListAsync();
    }

    public async Task<LaptopViewModel> GetLaptopById(int id)
    {
        var laptop = await _context.Product
       .Include(p => p.ProductImages) // Load hình ảnh
       .Include(p => p.Category) // Load danh mục
       .FirstOrDefaultAsync(p => p.Id == id);
        if (laptop == null)
        {
            throw new ArgumentNullException(nameof(laptop), "Product not found");
        }
        return _mapper.Map<LaptopViewModel>(laptop);
    }

    public async Task<IEnumerable<LaptopViewModel>> GetLaptops(string? sortOrder = null, string? currentFilter = null, string? searchString = null, int? pageNumber = null, int pageSize = 5)
    {
        var laptops = from p in _context.Product
                        .Include(p => p.Category)
                        .Include(p => p.ProductImages) // Bao gồm danh sách ảnh
                      select p;
        // Lọc theo từ khóa tìm kiếm nếu có
        if (!string.IsNullOrEmpty(searchString))
        {
            laptops = laptops.Where(p => p.Title != null &&
                                        p.Title.ToUpper().Contains(searchString.ToUpper()));
        }
        //Sắp xếp
        switch (sortOrder)
        {
            case "name_desc":
                laptops = laptops.OrderByDescending(p => p.Title);
                break;
            case "Price":
                laptops = laptops.OrderBy(p => p.Price);
                break;
            case "price_desc":
                laptops = laptops.OrderByDescending(p => p.Price);
                break;
            case "Quantity":
                laptops = laptops.OrderBy(p => p.Quantity);
                break;
            case "Quantity_desc":
                laptops = laptops.OrderByDescending(p => p.Quantity);
                break;
            default:
                laptops = laptops.OrderBy(p => p.Title);
                break;
        }
        // Chuyển đổi sang ViewModel
        return await PaginatedList<LaptopViewModel>.CreateAsync(
            laptops.Select(p => new LaptopViewModel
            {
                Id = p.Id,
                Title = p.Title,
                CategoryId = p.CategoryId,
                Name_Category = p.Category!.Name_Category,
                Quantity = p.Quantity,
                Price = p.Price,
                ImageUrl = p.ProductImages!.FirstOrDefault(img => img.IsMainImage)!.ImageUrl
            }),
        pageNumber ?? 1,
        pageSize);
    }

    public bool LaptopExists(int id)
    {
        return _context.Product.Any(e => e.Id == id);
    }

    public async Task<bool> Update(int id, LaptopViewModel laptop, IFormFile? MainImage)
    {
        var product = await _context.Product
        .Include(p => p.ProductImages)
        .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return false;
        }
        // Cập nhật thông tin sản phẩm
        _mapper.Map(laptop, product);
        // Xử lý ảnh mới nếu có
        if (MainImage != null && MainImage.Length > 0)
        {
            // Lưu ảnh mới
            var fileName = Path.GetFileNameWithoutExtension(MainImage.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(MainImage.FileName);
            var filePath = Path.Combine("wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await MainImage.CopyToAsync(stream);
            }

            // Xóa ảnh cũ
            var oldImage = product.ProductImages!.FirstOrDefault(img => img.IsMainImage);
            if (oldImage != null)
            {
                var oldImagePath = Path.Combine("wwwroot", oldImage.ImageUrl!.TrimStart('/'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
                _context.ProductImage!.Remove(oldImage); // Xóa khỏi database
            }

            // Thêm ảnh mới
            product.ProductImages!.Add(new ProductImage
            {
                ImageUrl = $"/images/{fileName}",
                IsMainImage = true
            });
        }

        // Lưu thay đổi
        _context.Update(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
public class CartService : ICartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MvcLaptopContext _context;

    public CartService(IHttpContextAccessor httpContextAccessor, MvcLaptopContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    // Lấy giỏ hàng từ Session
    public Dictionary<int, int> GetCartFromSession()
    {
        if (_httpContextAccessor.HttpContext?.Session == null)
        {
            throw new InvalidOperationException("Session is not available.");
        }
        var cartItemsJson = _httpContextAccessor.HttpContext.Session.GetString("CartItems");
        if (string.IsNullOrEmpty(cartItemsJson))
        {
            return new Dictionary<int, int>();
        }
        return System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(cartItemsJson) ?? new Dictionary<int, int>();
    }

    // Lưu giỏ hàng vào Session
    public void SaveCartToSession(Dictionary<int, int> cartItems)
    {
        if (_httpContextAccessor.HttpContext?.Session == null)
        {
            throw new InvalidOperationException("Session is not available.");
        }
        var cartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItems);
        _httpContextAccessor.HttpContext.Session?.SetString("CartItems", cartItemsJson);
    }

    // Thêm sản phẩm vào giỏ hàng
    public void AddToCart(int id, int quantity)
    {
        var cartItems = GetCartFromSession();
        if (cartItems.ContainsKey(id))// Sử dụng ContainsKey để kiểm tra khóa
        {
            cartItems[id] += quantity;
        }
        else
        {
            cartItems.Add(id, quantity);
        }
        SaveCartToSession(cartItems);
    }

    // Xóa sản phẩm khỏi giỏ hàng
    public void RemoveFromCart(int id)
    {
        var cartItems = GetCartFromSession();
        if (cartItems.ContainsKey(id))
        {
            cartItems.Remove(id);
        }
        SaveCartToSession(cartItems);
    }

    // Tính tổng tiền giỏ hàng
    public decimal CalculateTotalPrice(Dictionary<int, int> cartItems)
    {
        var cartProducts = cartItems.Select(ci => new
        {
            Products = _context.Product.FirstOrDefault(l => l.Id == ci.Key),
            Quantity = ci.Value
        }).Where(cp => cp.Products != null).ToList();

        return cartProducts.Sum(item => Convert.ToDecimal(item.Products!.Price) * Convert.ToDecimal(item.Quantity));
    }
}
