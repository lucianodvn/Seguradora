using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Seguradora.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace Seguradora.Api.IntegrationTests
{
    public sealed class ApiFactory : WebApplicationFactory<AssemblyMarker>, IAsyncLifetime
    {
        private readonly MsSqlContainer _sql = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
        public const string Usuario = "teste";
        public const string Senha = "senha-de-teste-123";
        private readonly RabbitMqContainer _rabbit = new RabbitMqBuilder("rabbitmq:3.13-management").Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.UseSetting("ConnectionStrings:Default", _sql.GetConnectionString());
            builder.UseSetting("Jwt:Key", "chave-de-teste-com-pelo-menos-32-caracteres!!");
            builder.UseSetting("Auth:Usuario", Usuario);
            builder.UseSetting("Auth:Senha", Senha);
            builder.UseSetting("Jwt:Issuer", "seguradora-testes");
            builder.UseSetting("Jwt:Audience", "seguradora-testes");
            builder.UseSetting("RabbitMq:Host", _rabbit.Hostname);
            builder.UseSetting("RabbitMq:Port",
                _rabbit.GetMappedPublicPort(5672).ToString(CultureInfo.InvariantCulture));
            builder.UseSetting("RabbitMq:Username", RabbitMqBuilder.DefaultUsername);
            builder.UseSetting("RabbitMq:Password", RabbitMqBuilder.DefaultPassword);
        }
        public async Task InitializeAsync()
        {
            await Task.WhenAll(_sql.StartAsync(), _rabbit.StartAsync());

            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SeguradoraDbContext>();
            await db.Database.MigrateAsync();
        }
        public new async Task DisposeAsync()
        {
            await _rabbit.DisposeAsync();
            await _sql.DisposeAsync();
        }

    }
}
