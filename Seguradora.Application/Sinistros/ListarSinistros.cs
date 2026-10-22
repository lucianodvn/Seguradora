using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Sinistros
{
    public sealed record ListarSinistrosQuery(
    Guid? ApoliceId,
    string? Status,
    DateTime? AbertoDesde,
    DateTime? AbertoAte,
    int Pagina = 1,
    int TamanhoPagina = 20);
}
