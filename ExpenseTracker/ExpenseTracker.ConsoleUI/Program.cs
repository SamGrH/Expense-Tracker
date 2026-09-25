using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Domain.Entities;
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
        Console.WriteLine("=== EXPENSE TRACKER - INTELIGENCIA ARTIFICIAL ===\n");

        // 1. Cargar la configuración desde User Secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        // 2. Configurar el Contenedor de Inyección de Dependencias
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // 3. Crear el Scope y asegurar la base de datos al día
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        // 4. Obtener el servicio de IA a través de la interfaz
        var aiCategorizer = scope.ServiceProvider.GetRequiredService<IAiCategorizerService>();

        // 5. Lista de consumos de prueba (claros + el caso dudoso "HERRERO SRL")
        var itemsToClassify = new List<string>
        {
            "Coto Sucursal 14",
            "Steam Games Purchase",
            "YPF Estacion 42",
            "HERRERO SRL",
            "Farmacity 210",
            "Netflix Mensual"
        };

        Console.WriteLine("🤖 Enviando consumos a Gemini AI (Modelo: 3.5 Flash Lite)...\n");
        var classifiedItems = await aiCategorizer.CategorizeExpensesAsync(itemsToClassify);

        // 6. Procesar los resultados (con soporte Human-in-the-Loop)
        Console.WriteLine("=== RESULTADOS DE LA CLASIFICACIÓN ===");
        foreach (var item in classifiedItems)
        {
            if (item.RequiresReview)
            {
                // Caso dudoso detectado: se solicita revisión manual
                Console.WriteLine($"\n⚠️ REVISIÓN MANUAL REQUERIDA:");
                Console.WriteLine($"   Consumo: \"{item.Description}\"");
                Console.WriteLine($"   Sugerencia IA: {item.CategoryName} (Certeza: {item.Confidence})");
                Console.Write($"   Presiona [ENTER] para aceptar '{item.CategoryName}' o escribe la categoría correcta: ");

                var input = Console.ReadLine();
                var finalCategory = string.IsNullOrWhiteSpace(input) ? item.CategoryName : input.Trim();
                Console.WriteLine($"   -> Categoría definitiva asignada: [{finalCategory}]\n");
            }
            else
            {
                // Caso seguro: categorizado automáticamente
                Console.WriteLine($" [Automático] {item.Description.PadRight(30)} -> {item.CategoryName.PadRight(18)} (Certeza: {item.Confidence})");
            }
        }

        Console.WriteLine("\n=== CIRCUITO DE IA COMPLETADO EXITOSAMENTE ===");
    }
}