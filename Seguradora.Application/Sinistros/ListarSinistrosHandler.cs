using Seguradora.Application.Abstractions;
using Seguradora.Application.Common;
using Seguradora.Domain.Sinistros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Sinistros
{
    internal sealed class ListarSinistrosHandler(ISinistroConsultas consultas)
        : IQueryHandler<ListarSinistrosQuery, PaginaResultado<SinistroDto>>
    {
        private const int TamanhoMaximoPagina = 100;
        public async Task<PaginaResultado<SinistroDto>> HandleAsync(
            ListarSinistrosQuery query, CancellationToken cancellationToken)
        {
            if (query.Pagina < 1)
                throw new EntradaInvalidaException("A página deve ser maior ou igual a 1.");
            if (query.TamanhoPagina < 1 || query.TamanhoPagina > TamanhoMaximoPagina)
                throw new EntradaInvalidaException($"O tamanho da página deve estar entre 1 e {TamanhoMaximoPagina}.");
            if (query.AbertoDesde > query.AbertoAte)
                throw new EntradaInvalidaException("'abertoDesde' não pode ser posterior a 'abertoAte'.");

            StatusSinistro? status = null;
            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                // IsDefined barra números soltos como "99", que o TryParse aceitaria
                if (!Enum.TryParse<StatusSinistro>(query.Status, ignoreCase: true, out var valor) || !Enum.IsDefined(valor))
                    throw new EntradaInvalidaException($"Status inválido: '{query.Status}'.");
                status = valor;
            }

            var filtro = new FiltroSinistros(
                query.ApoliceId, status, query.AbertoDesde, query.AbertoAte, query.Pagina, query.TamanhoPagina);

            return await consultas.ListarAsync(filtro, cancellationToken);
        }
    }
}
