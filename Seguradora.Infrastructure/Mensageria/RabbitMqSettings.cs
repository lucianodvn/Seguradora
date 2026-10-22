using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Infrastructure.Mensageria
{
    public sealed class RabbitMqSettings
    {
        public const string Secao = "RabbitMq";

        [Required] 
        public string Host { get; init; } = "";
        public ushort Port { get; init; } = 5672;
        public string VirtualHost { get; init; } = "/";

        [Required] 
        public string Username { get; init; } = "";

        [Required] 
        public string Password { get; init; } = "";
    }
}
