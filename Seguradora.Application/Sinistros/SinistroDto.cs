using Seguradora.Domain.Sinistros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Sinistros
{
    public sealed record SinistroDto(
    Guid Id,
    Guid ApoliceId,
    string Descricao,
    decimal ValorEstimado,
    DateTime DataOcorrencia,
    DateTime DataAbertura,
    string Status,
    string? MotivoNegativa);

    internal static class SinistroMappings
    {
        internal static SinistroDto ParaDto(this Sinistro sinistro) => new(
            sinistro.Id,
            sinistro.ApoliceId,
            sinistro.Descricao,
            sinistro.ValorEstimado,
            sinistro.DataOcorrencia,
            sinistro.DataAbertura,
            sinistro.Status.ToString(),
            sinistro.MotivoNegativa);
    }
}
