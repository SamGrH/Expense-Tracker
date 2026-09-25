using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.ConsoleUI;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== INICIANDO EXPENSE TRACKER ===\n");

        var services = new ServiceCollection();
        services.AddInfrastructureServices();
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var repository = scope.ServiceProvider.GetRequiredService<IExpenseRepository>();

        await dbContext.Database.MigrateAsync();

        if (!dbContext.Categories.Any())
        {
            var initialCategory = new Category(1, "Alimentos");
            dbContext.Categories.Add(initialCategory);
            await dbContext.SaveChangesAsync();
            Console.WriteLine($"Categoria inicial '{initialCategory.Name}' agregada a la base de datos.");

        }

        var newExpense = new Expense(
           id: 0,
           description: "Supermercado Coto",
           amount: 15420.50m,
           date: DateTime.Now,
           categoryId: 1
       );


        Console.WriteLine($"\n Guardando gasto: '{newExpense.Description}' por ${newExpense.Amount}...");
        await repository.AddAsync(newExpense);
        Console.WriteLine(" Gasto guardado con éxito en SQLite!");

        Console.WriteLine("\n=== LISTADO DE GASTOS DESDE SQLITE ===");
        var expenses = await repository.GetAllAsync();

        foreach (var expense in expenses)
        {
            Console.WriteLine($"- [ID: {expense.Id}] {expense.Date:dd/MM/yyyy HH:mm} | {expense.Description} | ${expense.Amount:N2} | Categoría: {expense.Category.Name}");
        }

        Console.WriteLine("\n=== CIRCUITO COMPLETADO EXITOSAMENTE ===");
    }
}

