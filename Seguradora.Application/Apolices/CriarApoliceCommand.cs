using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Apolices
{
    public sealed record CriarApoliceCommand(
        string Segurado,
        decimal ValorCobertura,
        DateTime VigenciaInicio,
        DateTime VigenciaFim);
}
