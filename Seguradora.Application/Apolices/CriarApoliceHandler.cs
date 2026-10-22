using Seguradora.Application.Abstractions;
using Seguradora.Domain.Apolices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Apolices
{
    internal sealed class CriarApoliceHandler(IApoliceRepository apolices, IUnitOfWork unitOfWork)
        : ICommandHandler<CriarApoliceCommand, ApoliceDto>
    {
        public async Task<ApoliceDto> HandleAsync(CriarApoliceCommand command, CancellationToken cancellationToken)
        {
            var apolice = Apolice.Criar(
                command.Segurado, command.ValorCobertura, command.VigenciaInicio, command.VigenciaFim);

            await apolices.AdicionarAsync(apolice, cancellationToken);
            await unitOfWork.SalvarAsync(cancellationToken);

            return apolice.ParaDto();
        }
    }
}
