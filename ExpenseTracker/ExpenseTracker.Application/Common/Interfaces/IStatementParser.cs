using ExpenseTracker.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Common.Interfaces;

public interface IStatementParser
{
    Task<IEnumerable<RawTransactionDto>> ParseAsync(string filePath, CancellationToken cancellationToken = default);

}

