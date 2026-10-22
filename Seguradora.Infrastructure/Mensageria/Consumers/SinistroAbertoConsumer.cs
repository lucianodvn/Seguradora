using MassTransit;
using Microsoft.Extensions.Logging;
using Seguradora.Application.Eventos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Infrastructure.Mensageria.Consumers
{
    internal sealed partial class SinistroAbertoConsumer(ILogger<SinistroAbertoConsumer> logger)
     : IConsumer<SinistroAberto>
    {
        public Task Consume(ConsumeContext<SinistroAberto> context)
        {
            var m = context.Message;
            LogSinistroRecebido(logger, m.SinistroId, m.ApoliceId, m.ValorEstimado);

            return Task.CompletedTask;
        }

        [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Sinistro {SinistroId} da apólice {ApoliceId} recebido para análise (valor estimado {Valor}).")]
        private static partial void LogSinistroRecebido(ILogger logger, Guid sinistroId, Guid apoliceId, decimal valor);
    }
}
