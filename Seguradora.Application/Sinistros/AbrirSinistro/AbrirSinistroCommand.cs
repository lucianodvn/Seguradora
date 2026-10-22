using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Sinistros.AbrirSinistro
{
    public sealed record AbrirSinistroCommand(
    Guid ApoliceId,
    string Descricao,
    decimal ValorEstimado,
    DateTime DataOcorrencia);
}
