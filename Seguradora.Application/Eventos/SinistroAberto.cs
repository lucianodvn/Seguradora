using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Eventos
{
    public sealed record SinistroAberto
    (
        Guid SinistroId,
        Guid ApoliceId,
        decimal ValorEstimado,
        DateTimeOffset OcorridoEm
    );
}
