using MassTransit;
using Seguradora.Application.Eventos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Infrastructure.Mensageria
{
    internal sealed class PublicadorEventosMassTransit(IPublishEndpoint publish) : IPublicadorEventos
    {
        public Task PublicarAsync<T>(T evento, CancellationToken ct) where T : class
            => publish.Publish(evento, ct);
    }
}
