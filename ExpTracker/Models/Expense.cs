
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpTracker.Models
{
    public class Expense : IValidatableObject
    {
        [Key]
        public int Id { get; set; }
        public double Amount { get; set; }
    
        public Category? Category { get; set; }
        public AppUser? User { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            if (Date > DateTime.Now)
            {
                results.Add(new ValidationResult("Date cannot be in the future"));
            }
            return results;
        }
    }
}
