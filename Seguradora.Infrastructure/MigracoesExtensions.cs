using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Seguradora.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Infrastructure
{
    public static class MigracoesExtensions
    {
        public static async Task AplicarMigracoesAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SeguradoraDbContext>();
            await db.Database.MigrateAsync();
        }
    }
}
