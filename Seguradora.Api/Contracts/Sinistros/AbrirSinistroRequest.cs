using System.ComponentModel.DataAnnotations;

namespace Seguradora.Api.Contracts.Sinistros
{
    public sealed record AbrirSinistroRequest(
    [Required, StringLength(1000)] string Descricao,
    [Range(0.01, 1_000_000_000)] decimal ValorEstimado,
    DateTimeOffset DataOcorrencia);
}
