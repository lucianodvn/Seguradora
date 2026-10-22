using Seguradora.Domain.Apolices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Apolices
{
    public sealed record ApoliceDto(
        Guid Id,
        string Segurado,
        decimal ValorCobertura,
        DateTime VigenciaInicio,
        DateTime VigenciaFim,
        string Status);

    internal static class ApoliceMappings
    {
        internal static ApoliceDto ParaDto(this Apolice apolice) => new(
            apolice.Id,
            apolice.Segurado,
            apolice.ValorCobertura,
            apolice.VigenciaInicio,
            apolice.VigenciaFim,
            apolice.Status.ToString());
    }
}
