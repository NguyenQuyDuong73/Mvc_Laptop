using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Data;
using MvcLaptop.Models;
using AutoMapper;
using MvcLaptop.Services;

namespace MvcLaptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LaptopsController : Controller
    {
        private readonly ILaptopService _laptopService;
        public LaptopsController(ILaptopService laptopService)
        {
            _laptopService = laptopService;
        }

        // GET: Laptops
        public async Task<IActionResult> Index(string searchString)
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            ViewData["SearchString"] = searchString;
            // Lấy danh sách laptop
            return View(await _laptopService.GetLaptops(searchString));
        }
        
        // GET: Laptops/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            if (id == null)
            {
                return NotFound();
            }

            var laptop = await _laptopService.GetLaptopById(id.Value);
            if (laptop == null)
            {
                return NotFound();
            }
            return View(laptop);
        }

        // GET: Laptops/Create
        public async Task<IActionResult> Create()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = userName;
            ViewBag.Categories = new SelectList(await _laptopService.GetCategories(), "CategoryId", "Name_Category");
            return View();
        }

        // POST: Laptops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LaptopRequest request, IFormFile MainImage)
        {
            if (ModelState.IsValid)
            {
                var result = await _laptopService.Create(request, MainImage);
                if(result == null)
                    return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _laptopService.GetCategories(), "CategoryId", "Name_Category", request.CategoryId);
            return View(request);
        }

        // GET: Laptops/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var laptop = await _laptopService.GetLaptopById(id.Value);
            if (laptop == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(await _laptopService.GetCategories(), "CategoryId", "Name_Category", laptop.CategoryId);
            return View(laptop);
        }

        // POST: Laptops/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,LaptopViewModel laptop, IFormFile? MainImage)
        {
            if (id != laptop.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _laptopService.Update(id, laptop, MainImage);
                    if(result)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_laptopService.LaptopExists(laptop.Id))
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
            ViewBag.Categories = new SelectList(await _laptopService.GetCategories(), "CategoryId", "Name_Category", laptop.CategoryId);
            return View(laptop);
        }

        // GET: Laptops/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laptop = await _laptopService.GetLaptopById(id.Value);
            if (laptop == null)
            {
                return NotFound();
            }

            return View(laptop);
        }

        // POST: Laptops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _laptopService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
