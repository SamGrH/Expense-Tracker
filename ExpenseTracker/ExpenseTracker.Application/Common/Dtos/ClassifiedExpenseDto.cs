using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Common.Dtos;

public record ClassifiedExpenseDto
(
    string Description,
    string CategoryName,
    string Confidence,
    bool RequiresReview
);


