using ExpenseTracker.Application.Common.Dtos;
using ExpenseTracker.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Infrastructure.Parsers
{
    public class SimpleTextStatementParser : IStatementParser
    {
        public async Task<IEnumerable<RawTransactionDto>> ParseAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file '{filePath}' does not exist.");
            }

            var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
            var transactions = new List<RawTransactionDto>();

            for (int i = 0; i < lines.Length;i++)
            {
                var line = lines[i].Trim();

                // Saltear líneas vacías o comentarios que empiecen con '#'
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }
                var parts = line.Split(';');
                if (parts.Length < 3)
                {
                    //si la linea no tiene el formato esperado, se puede registrar un error o lanzar una excepción
                    continue; // Línea inválida
                }

                var rawDate = parts[0].Trim();
                var description = parts[1].Trim();
                var rawAmount = parts[2].Trim();

                // Parseo flexible de fechas (admite yyyy-MM-dd y dd/MM/yyyy)

                if (!DateTime.TryParse(rawDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) &&
                                !DateTime.TryParseExact(rawDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    continue; // Fecha inválida
                }

                // Parseo flexible de montos (soporta punto o coma decimal)
                var normalizedAmount = rawAmount.Replace(',', '.');
                if (!decimal.TryParse(normalizedAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                {
                    continue; // Monto inválido
                }
                transactions.Add(new RawTransactionDto(description, amount, date));
            }
            return transactions;
        }
    }
}
