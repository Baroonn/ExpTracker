using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpTracker.Data;
using ExpTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExpTracker.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CategoryController(ApplicationDbContext context, 
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var user = await GetUserAsync();
            return View(await _context.Categories.Where(x => x.User == user).ToListAsync());
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await GetUserAsync();
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.Where(x => x.User == user && x.Id == id)
                .FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            category.User = await GetUserAsync();
            //Check if the category exists
            var categories = _context.Categories.Where(x => x.User == category.User && x.Name == category.Name);
            if (categories.Any())
            {
                ModelState.AddModelError("Not Unique", "Cannot add duplicate category");
            }
            if (ModelState.IsValid)
            { 
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await GetUserAsync();
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.Where(x => x.User == user && x.Id == id).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            var user = await GetUserAsync();
            if (id != category.Id)
            {
                return NotFound();
            }
            var categories = _context.Categories.Where(x => x.User == user && x.Name == category.Name);
            if (categories.Any())
            {
                ModelState.AddModelError("Not Unique", "Cannot add duplicate category");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await GetUserAsync();
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.Where(x => x.User == user && x.Id == id).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await GetUserAsync();
            var category = await _context.Categories.Where(x => x.Id == id && x.User == user).FirstOrDefaultAsync();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        private async Task<AppUser> GetUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
