using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Eventos
{
    public interface IPublicadorEventos
    {
        Task PublicarAsync<T>(T evento, CancellationToken ct) where T : class;
    }
}
