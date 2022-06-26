using ExpTracker.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpTracker.Models
{
    public class Category 
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public AppUser? User { get; set; }
        public ICollection<Expense>? Expenses { get; set; }

    }
}
