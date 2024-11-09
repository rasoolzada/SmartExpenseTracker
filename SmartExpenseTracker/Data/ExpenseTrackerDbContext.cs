using SmartExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace SmartExpenseTracker.Data
{
    public class ExpenseTrackerDbContext:DbContext
    {
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options) : base(options) { }
    }
}
