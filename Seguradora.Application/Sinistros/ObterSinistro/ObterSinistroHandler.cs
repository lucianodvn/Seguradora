using Seguradora.Application.Abstractions;
using Seguradora.Application.Common;
using Seguradora.Application.Sinistros;
using Seguradora.Domain.Sinistros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Sinistros.ObterSinistro
{
    internal sealed class ObterSinistroHandler(ISinistroRepository sinistros)
        : IQueryHandler<ObterSinistroQuery, SinistroDto>
    {
        public async Task<SinistroDto> HandleAsync(ObterSinistroQuery query, CancellationToken cancellationToken)
        {
            var sinistro = await sinistros.ObterPorIdAsync(query.Id, cancellationToken)
                ?? throw new NaoEncontradoException($"Sinistro {query.Id} não encontrado.");

            return sinistro.ParaDto();
        }
    }
}
