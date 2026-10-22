using Seguradora.Application.Abstractions;
using Seguradora.Application.Common;
using Seguradora.Application.Eventos;
using Seguradora.Domain.Apolices;
using Seguradora.Domain.Sinistros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Sinistros.AbrirSinistro
{
    internal sealed class AbrirSinistroHandler(
        IApoliceRepository apolices,
        ISinistroRepository sinistros,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        IPublicadorEventos publicador)
    : ICommandHandler<AbrirSinistroCommand, SinistroDto>
    {
        public async Task<SinistroDto> HandleAsync(AbrirSinistroCommand command, CancellationToken cancellationToken)
        {
            var apolice = await apolices.ObterPorIdAsync(command.ApoliceId, cancellationToken)
                ?? throw new NaoEncontradoException($"Apólice {command.ApoliceId} não encontrada.");

            var agora = timeProvider.GetUtcNow().UtcDateTime;

            // As regras (vigência, cobertura, datas) são do Domain
            var sinistro = apolice.AbrirSinistro(
                command.Descricao, command.ValorEstimado, command.DataOcorrencia, agora);

            await sinistros.AdicionarAsync(sinistro, cancellationToken);

            await publicador.PublicarAsync(
                new SinistroAberto(sinistro.Id, apolice.Id, sinistro.ValorEstimado, sinistro.DataOcorrencia), cancellationToken);

            await unitOfWork.SalvarAsync(cancellationToken);

            return sinistro.ParaDto();
        }
    }
}
