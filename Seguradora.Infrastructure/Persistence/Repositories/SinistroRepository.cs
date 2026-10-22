using Microsoft.EntityFrameworkCore;
using Seguradora.Domain.Sinistros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Infrastructure.Persistence.Repositories
{
    internal sealed class SinistroRepository(SeguradoraDbContext context) : ISinistroRepository
    {
        public Task<Sinistro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) => context.Sinistros.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task AdicionarAsync(Sinistro sinistro, CancellationToken cancellationToken) => await context.Sinistros.AddAsync(sinistro, cancellationToken);
    }
}
