using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ExpTracker.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Category> Categories { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
}
