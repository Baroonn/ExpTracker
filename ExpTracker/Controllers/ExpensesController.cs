using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpTracker.Data;
using ExpTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ExpTracker.Controllers
{
    [Authorize]
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ExpensesController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Expenses
        public async Task<IActionResult> Index(DateTime from, DateTime to)
        {
            //Check if a range was provided: If not from eq default and to eq current date
            if (to == default)
            {
                to = DateTime.Now;
            }
            var user = await GetUserAsync();
            return View(await _context.Expenses.Where(x => x.User == user && from <= x.Date && to >= x.Date).ToListAsync());
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await GetUserAsync();
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses.Where(e => e.Id == id && e.User == user)
                .FirstOrDefaultAsync();
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expenses/Create
        public async Task<IActionResult> Create()
        {
            var user = await GetUserAsync();
            var categories = _context.Categories.Where(x => x.User == user).ToList();

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();

        }

        // POST: Expenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            var user = await GetUserAsync();
            var categoryId = Int32.Parse(Request.Form["Category"]);
            //Confirm that the category exists for the user
            var category = await _context.Categories.Where(x => x.Id == categoryId && x.User == user).FirstOrDefaultAsync();
            if(category != null)
            {
                expense.User = user;
                expense.Category = category;
            }
            

            if (ModelState.IsValid)
            {
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await GetUserAsync();
            if (id == null)
            {
                return NotFound();
            }

            //Confirm that the user owns the expense
            var expense = await _context.Expenses.Where(x => x.Id == id && x.User == user).FirstOrDefaultAsync();
            if (expense == null)
            {
                return NotFound();
            }
            var categories = _context.Categories.Where(x => x.User == user).ToList();

            ViewBag.Categories = new SelectList(categories, "Id", "Name", "3");
            
            return View(expense);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            var user = await GetUserAsync();
            if (id != expense.Id)
            {
                return NotFound();
            }
           
            var categoryId = Int32.Parse(Request.Form["Category"]);
            var category = await _context.Categories.Where(x => x.Id == categoryId && x.User == user).FirstOrDefaultAsync();
            if (category != null)
            {
                expense.Category = category;
                expense.UpdatedAt = DateTime.Now;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.Id))
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
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await GetUserAsync();
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses.Where(x => x.Id == id && x.User == user)
                .FirstOrDefaultAsync();
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await GetUserAsync();
            var expense = await _context.Expenses.Where(x => x.Id == id && x.User == user).FirstOrDefaultAsync();
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }

        private async Task<AppUser> GetUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
