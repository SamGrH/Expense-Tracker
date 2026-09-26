using ExpenseTracker.Application.Common.Dtos;
using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.UseCases;

public class ImportStatementUseCase
{
    private readonly IStatementParser _parser;
    private readonly IAiCategorizerService _aiCategorizerService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IExpenseRepository _expenseRepository;

    public ImportStatementUseCase(
        IStatementParser parser,
        IAiCategorizerService aiCategorizerService,
        ICategoryRepository categoryRepository,
        IExpenseRepository expenseRepository)
    {
        _parser = parser;
        _aiCategorizerService = aiCategorizerService;
        _categoryRepository = categoryRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<int> ExecuteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        // 1. Parse statement
        var rawTransactions = await _parser.ParseAsync(filePath, cancellationToken);
        if (!rawTransactions.Any())
        {
            return 0;
        }

        // 2. Fetch existing categories
        var existingCategoriesEntities = await _categoryRepository.GetAllAsync(cancellationToken);
        var existingCategoriesNames = existingCategoriesEntities.Select(c => c.Name).ToList();

        // 3. Ask AI to categorize
        var descriptionsToCategorize = rawTransactions.Select(t => t.Description).Distinct().ToList();

        var categorizedExpenses = await _aiCategorizerService.CategorizeExpensesAsync(
            descriptionsToCategorize,
            existingCategoriesNames,
            cancellationToken);

        // Map AI result by Description for quick lookup
        var categoryMap = categorizedExpenses
            .GroupBy(c => c.Description)
            .ToDictionary(g => g.Key, g => g.First().CategoryName);

        int importedCount = 0;

        // 4. Process each transaction
        foreach (var rawTx in rawTransactions)
        {
            string categoryName = categoryMap.ContainsKey(rawTx.Description)
                ? categoryMap[rawTx.Description]
                : "Uncategorized"; // Default if AI fails to return it

            // Check if category already exists (maybe we just created it in this loop, so we fetch again from our local list)
            var categoryEntity = existingCategoriesEntities.FirstOrDefault(c => c.Name.Equals(categoryName, System.StringComparison.OrdinalIgnoreCase));

            if (categoryEntity == null)
            {
                // Create and save new category
                categoryEntity = new Category(categoryName);
                await _categoryRepository.AddAsync(categoryEntity, cancellationToken);

                // Add to our local list so we don't recreate it if another tx has the same new category
                var list = existingCategoriesEntities.ToList();
                list.Add(categoryEntity);
                existingCategoriesEntities = list;
            }

            // 5. Create Expense and save
            var expense = new Expense(rawTx.Description, rawTx.Amount, rawTx.Date, categoryEntity.Id);
            await _expenseRepository.AddAsync(expense, cancellationToken);
            importedCount++;
        }

        return importedCount;
    }
}