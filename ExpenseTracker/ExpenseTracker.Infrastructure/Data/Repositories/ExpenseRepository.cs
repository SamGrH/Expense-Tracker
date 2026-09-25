using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Infrastructure.Data.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ExpenseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            await _dbContext.Expenses.AddAsync(expense, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

        }

        public async Task<IEnumerable<Expense>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Expenses
                .Include(e => e.Category)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
