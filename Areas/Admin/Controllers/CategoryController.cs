using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Authorization;
using MvcLaptop.Data;
using MvcLaptop.Models;
using MvcLaptop.Utils.Constants;
using System.Linq;
using System.Threading.Tasks;

namespace MvcLaptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly MvcLaptopContext _context;

        public CategoryController(MvcLaptopContext context)
        {
            _context = context;
        }
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
        public IActionResult Index()
        {
            var categories = _context.Category!.ToList();
            return View(categories);
        }
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
        // Trang chi tiết Category (GET)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category!
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // Trang tạo Category mới (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Trang tạo Category mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.CREATE)]
        public async Task<IActionResult> Create([Bind("CategoryId, Name_Category, Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // Trang sửa Category (GET)
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.UPDATE)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category!.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Trang sửa Category (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId, Name_Category, Description")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = await _context.Category!.FindAsync(id);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường
                    existingCategory.Name_Category = category.Name_Category;
                    existingCategory.Description = category.Description;

                    _context.Update(existingCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // Trang xóa Category (GET)
        [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.DELETE)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category!
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // Trang xóa Category (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Category!.FindAsync(id);
            if (category != null)
            {
                _context.Category.Remove(category);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category!.Any(e => e.CategoryId == id);
        }
    }
}