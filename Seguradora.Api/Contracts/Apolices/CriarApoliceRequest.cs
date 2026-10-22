using System.ComponentModel.DataAnnotations;

namespace Seguradora.Api.Contracts.Apolices
{
    public sealed record CriarApoliceRequest(
    [Required, StringLength(200)] string Segurado,
    [Range(0.01, 1_000_000_000)] decimal ValorCobertura,
    DateTimeOffset VigenciaInicio,
    DateTimeOffset VigenciaFim);
}
