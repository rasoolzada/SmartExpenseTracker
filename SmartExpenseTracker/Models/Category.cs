using SmartExpenseTracker.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SmartExpenseTracker.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Expense> Expenses { get; set; }
    }
}
