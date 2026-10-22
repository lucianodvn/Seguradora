using Microsoft.AspNetCore.Mvc;
using Seguradora.Api.Contracts.Sinistros;
using Seguradora.Application.Abstractions;
using Seguradora.Application.Common;
using Seguradora.Application.Sinistros;
using Seguradora.Application.Sinistros.AbrirSinistro;
using Seguradora.Application.Sinistros.ObterSinistro;

namespace Seguradora.Api.Controllers
{
    [ApiController]
    [Route("api/sinistros")]
    public class SinistrosController : ControllerBase
    {
        [HttpPost("~/api/apolices/{apoliceId:guid}/sinistros")]
        [ProducesResponseType<SinistroDto>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<SinistroDto>> Abrir(
        Guid apoliceId,
        AbrirSinistroRequest request,
        [FromServices] ICommandHandler<AbrirSinistroCommand, SinistroDto> handler,
        CancellationToken cancellationToken)
        {
            var comando = new AbrirSinistroCommand(
                apoliceId,
                request.Descricao,
                request.ValorEstimado,
                request.DataOcorrencia.UtcDateTime);

            var sinistro = await handler.HandleAsync(comando, cancellationToken);

            return CreatedAtAction(nameof(Obter), new { id = sinistro.Id }, sinistro);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType<SinistroDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SinistroDto>> Obter(
            Guid id,
            [FromServices] IQueryHandler<ObterSinistroQuery, SinistroDto> handler,
            CancellationToken cancellationToken)
        {
            var sinistro = await handler.HandleAsync(new ObterSinistroQuery(id), cancellationToken);
            return Ok(sinistro);
        }

        [HttpGet]
        [ProducesResponseType<PaginaResultado<SinistroDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginaResultado<SinistroDto>>> Listar(
            [FromServices] IQueryHandler<ListarSinistrosQuery, PaginaResultado<SinistroDto>> handler,
            CancellationToken cancellationToken,
            [FromQuery] Guid? apoliceId = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTimeOffset? abertoDesde = null,
            [FromQuery] DateTimeOffset? abertoAte = null,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 20)
        {
            var query = new ListarSinistrosQuery(
                apoliceId, status, abertoDesde?.UtcDateTime, abertoAte?.UtcDateTime, pagina, tamanhoPagina);

            return Ok(await handler.HandleAsync(query, cancellationToken));
        }
    }
}

