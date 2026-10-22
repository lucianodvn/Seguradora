using Seguradora.Application.Common;
using Seguradora.Domain.Sinistros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Sinistros
{
    public interface ISinistroConsultas
    {
        Task<PaginaResultado<SinistroDto>> ListarAsync(FiltroSinistros filtro, CancellationToken cancellationToken);
    }

    public sealed record FiltroSinistros(Guid? ApoliceId,
    StatusSinistro? Status,
    DateTime? AbertoDesde,
    DateTime? AbertoAte,
    int Pagina,
    int TamanhoPagina);
}
