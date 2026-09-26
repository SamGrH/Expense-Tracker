using ExpenseTracker.Application.UseCases;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.ConsoleUI;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== EXPENSE TRACKER - ISSUE #5: IMPORT STATEMENT ===\n");

        // 1. Cargar User Secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        // 2. Configurar DI
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddInfrastructureServices(configuration);

        // Registramos el Caso de Uso directamente aquí:
        services.AddScoped<ImportStatementUseCase>();

        var serviceProvider = services.BuildServiceProvider();

        // 3. Asegurar Base de Datos al día
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        // 4. Crear un archivo de prueba 'statement.txt' si no existe
        var sampleFilePath = "statement.txt";

        if (!File.Exists(sampleFilePath))
        {
            var sampleContent =
                "# Extracto bancario de prueba\n" +
                "2026-09-25; Coto Sucursal 14; 15420.50\n" +
                "2026-09-25; Steam Games Purchase; 3499.00\n" +
                "2026-09-24; YPF Estacion 42; 25000,00\n" +
                "# Comentario que debe ignorar el parser\n" +
                "24/09/2026; Farmacity 210; 8950.25\n" +
                "2026-09-22; Netflix Mensual; 7500.00\n";

            await File.WriteAllTextAsync(sampleFilePath, sampleContent);
            Console.WriteLine($"📄 Archivo de prueba creado: '{sampleFilePath}'\n");
        }

        // 5. Ejecutar el Caso de Uso
        var importUseCase = scope.ServiceProvider.GetRequiredService<ImportStatementUseCase>();

        Console.WriteLine("🚀 Iniciando importación y categorización con IA...\n");
        var importedCount = await importUseCase.ExecuteAsync(sampleFilePath);

        Console.WriteLine($"\n✅ Se importaron {importedCount} transacciones exitosamente.\n");

        // 6. Verificación en Base de Datos (DoD)
        Console.WriteLine("=== GASTOS REGISTRADOS EN LA BASE DE DATOS ===");
        var expensesInDb = await dbContext.Expenses
            .Include(e => e.Category)
            .OrderByDescending(e => e.Date)
            .ToListAsync();

        foreach (var expense in expensesInDb)
        {
            Console.WriteLine($"[{expense.Date:yyyy-MM-dd}] {expense.Description.PadRight(25)} | ${expense.Amount,10:N2} | Categoría: {expense.Category.Name}");
        }

        Console.WriteLine("\n=== FIN DEL CIRCUITO ===");
    }
}