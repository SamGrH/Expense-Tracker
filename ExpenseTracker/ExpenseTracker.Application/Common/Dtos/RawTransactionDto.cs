using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Common.Dtos;

    public record RawTransactionDto
    (
        string Description,
        decimal Amount,
        DateTime Date
    );
    
    

