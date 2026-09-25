using ExpenseTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Common.Interfaces;

public interface IExpenseRepository
{
    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);
    Task <IEnumerable <Expense>> GetAllAsync(CancellationToken cancellationToken = default);



}

