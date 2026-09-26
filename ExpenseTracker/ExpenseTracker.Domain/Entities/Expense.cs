using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Domain.Entities
{
    public class Expense
    {
        public int Id { get;private set; }
        public string Description { get;private set; } = null!;
        public decimal Amount { get;private set; }
        public DateTime Date { get;private set; }
        public int CategoryId { get;private set; }
        public Category Category { get; private set; } = null!;

        private Expense() { }


        public Expense(int id, string description, decimal amount, DateTime date, int categoryId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description);
            
            if (description.Length > 200)
            {
                throw new ArgumentOutOfRangeException(nameof(description), "Description cannot exceed 200 characters.");
            }

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(categoryId);

            Id = id;
            Description = description;
            Amount = amount;
            Date = date;
            CategoryId = categoryId;
        }
        public Expense(string description, decimal amount, DateTime date, int categoryId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(description);

            if (description.Length > 200)
            {
                throw new ArgumentOutOfRangeException(nameof(description), "Description cannot exceed 200 characters.");
            }
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(categoryId);
            Description = description;
            Amount = amount;
            Date = date;
            CategoryId = categoryId;
        }


    }
}
