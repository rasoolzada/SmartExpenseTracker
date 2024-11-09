using Microsoft.AspNetCore.Mvc;
using SmartExpenseTracker.Data;
using SmartExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
using SmartExpenseTracker.Services;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartExpenseTracker.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly SpendingPredictionService _predictionService;

        public ExpensesController(ExpenseTrackerDbContext context, SpendingPredictionService predictionService)
        {
            _context = context;
            _predictionService = predictionService;
        }

        public async Task<IActionResult> Index()
        {
            var expenses = await _context.Expenses.Include(e => e.Category).ToListAsync();
            return View(expenses);
        }

        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Expense expense)
        {
            expense.CategoryId = GetCategoryBasedOnDescription(expense.Description);
            _context.Add(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", expense.CategoryId);
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (id != expense.Id) return NotFound();

                try
                {
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Expenses.Any(e => e.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));

            //ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", expense.CategoryId);
            //return View(expense);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null) return NotFound();

            return View(expense);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult SpendingPrediction()
        {
            var prediction = _predictionService.PredictNextMonthSpending();
            ViewBag.Prediction = prediction;
            return View();
        }


        private int GetCategoryBasedOnDescription(string description)
        {
            // Simple categorization logic based on description
            if (description.ToLower().Contains("grocery")) return 1;  // Assume ID 1 is Groceries
            if (description.ToLower().Contains("utility")) return 2;  // Assume ID 2 is Utilities
            return 3;  // Default category ID
        }

        // Add Edit and Delete actions similarly for full CRUD support
    }
}
