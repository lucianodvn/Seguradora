using Microsoft.EntityFrameworkCore;
using Seguradora.Domain.Apolices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Infrastructure.Persistence.Repositories
{
    internal sealed class ApoliceRepository(SeguradoraDbContext context) : IApoliceRepository
    {
        public Task<Apolice?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) => context.Apolices.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task AdicionarAsync(Apolice apolice, CancellationToken cancellationToken) => await context.Apolices.AddAsync(apolice, cancellationToken);
    }
}
