using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Infrastructure.Configuration;
using ExpenseTracker.Infrastructure.Data;
using ExpenseTracker.Infrastructure.Data.Repositories;
using ExpenseTracker.Infrastructure.Parsers;
using ExpenseTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1. Base de datos SQLite
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("Data Source=ExpenseTracker.db"));

            // 2. Repositorio de Gastos
            services.AddScoped<IExpenseRepository, ExpenseRepository>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            // 3. Opciones de configuración para Gemini (Options Pattern)
            services.Configure<GeminiOptions>(configuration.GetSection(GeminiOptions.SectionName));

            // 4. Cliente HTTP con el servicio categorizador de Gemini
            services.AddHttpClient<IAiCategorizerService, GeminiCategorizerService>();

            services.AddScoped<IStatementParser,SimpleTextStatementParser>();

            return services;
        }
    }
}
