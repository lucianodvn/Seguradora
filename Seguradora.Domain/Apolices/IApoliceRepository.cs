using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Domain.Apolices
{
    public interface IApoliceRepository
    {
        Task<Apolice?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
        Task AdicionarAsync(Apolice apolice, CancellationToken cancellationToken);
    }
}
