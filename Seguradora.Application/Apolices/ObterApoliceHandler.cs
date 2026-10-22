using Seguradora.Application.Abstractions;
using Seguradora.Application.Common;
using Seguradora.Domain.Apolices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Apolices
{
    internal sealed class ObterApoliceHandler(IApoliceRepository repository) : IQueryHandler<ObterApoliceQuery, ApoliceDto>
    {
        public async Task<ApoliceDto> HandleAsync(ObterApoliceQuery query, CancellationToken cancellationToken)
        {
            var apolice = await repository.ObterPorIdAsync(query.Id, cancellationToken) ??
                throw new NaoEncontradoException($"Apólice com ID {query.Id} não encontrada.");

            return apolice.ParaDto();

        }
    }
}
