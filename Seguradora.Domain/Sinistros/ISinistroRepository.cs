using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Domain.Sinistros
{
    public interface ISinistroRepository
    {
        Task<Sinistro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
        Task AdicionarAsync(Sinistro sinistro, CancellationToken cancellationToken);
    }
}
