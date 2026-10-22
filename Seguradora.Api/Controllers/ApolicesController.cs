using Microsoft.AspNetCore.Mvc;
using Seguradora.Api.Contracts.Apolices;
using Seguradora.Application.Abstractions;
using Seguradora.Application.Apolices;
using Seguradora.Application.Sinistros;
using Seguradora.Application.Sinistros.ObterSinistro;

namespace Seguradora.Api.Controllers
{
    [ApiController]
    [Route("api/apolices")]
    public sealed class ApolicesController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType<ApoliceDto>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ApoliceDto>> Criar(CriarApoliceRequest request, [FromServices] ICommandHandler<CriarApoliceCommand, ApoliceDto> handler, CancellationToken cancellationToken)
        {
            var comando = new CriarApoliceCommand
            (
                request.Segurado,
                request.ValorCobertura,
                request.VigenciaInicio.UtcDateTime,
                request.VigenciaFim.UtcDateTime
            );

            var apoliceDto = await handler.HandleAsync(comando, cancellationToken);

            return CreatedAtAction(nameof(Obter), new { id = apoliceDto.Id }, apoliceDto);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType<ApoliceDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApoliceDto>> Obter(Guid id, [FromServices] IQueryHandler<ObterApoliceQuery, ApoliceDto> handler, CancellationToken cancellationToken)
        {
            var apolice = await handler.HandleAsync(new ObterApoliceQuery(id), cancellationToken);
            return Ok(apolice);
        }
    }
}
