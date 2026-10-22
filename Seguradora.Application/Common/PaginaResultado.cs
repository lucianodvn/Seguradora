using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Common
{
    public sealed record PaginaResultado<T>(IReadOnlyList<T> Itens, int Pagina, int TamanhoPagina, int Total)
    {
        public int TotalPaginas => (int)Math.Ceiling(Total / (double)TamanhoPagina);
    }
}
