using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Seguradora.Application.Abstractions;
using Seguradora.Application.Eventos;
using Seguradora.Application.Sinistros;
using Seguradora.Domain.Apolices;
using Seguradora.Domain.Sinistros;
using Seguradora.Infrastructure.Mensageria;
using Seguradora.Infrastructure.Mensageria.Consumers;
using Seguradora.Infrastructure.Persistence;
using Seguradora.Infrastructure.Persistence.Consultas;
using Seguradora.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("A connection string 'Default' não foi configurada.");

            services.AddDbContext<SeguradoraDbContext>(options =>
                options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SeguradoraDbContext>());
            services.AddScoped<IApoliceRepository, ApoliceRepository>();
            services.AddScoped<ISinistroRepository, SinistroRepository>();
            services.AddSingleton(new SqlConnectionFactory(connectionString));
            services.AddScoped<ISinistroConsultas, SinistroConsultas>();

            services.AddOptions<RabbitMqSettings>()
                .BindConfiguration(RabbitMqSettings.Secao)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<IPublicadorEventos, PublicadorEventosMassTransit>();

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumer<SinistroAbertoConsumer>();

                // NOVO: outbox transacional (as mensagens vão para o banco, na mesma transação)
                x.AddEntityFrameworkOutbox<SeguradoraDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                // NOVO: consumidores descartam mensagens duplicadas (inbox)
                x.AddConfigureEndpointsCallback((context, nome, cfg) =>
                {
                    cfg.UseEntityFrameworkOutbox<SeguradoraDbContext>(context);
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    var s = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                    cfg.Host(s.Host, s.Port, s.VirtualHost, h =>
                    {
                        h.Username(s.Username);
                        h.Password(s.Password);
                    });

                    cfg.UseMessageRetry(r => r.Exponential(
                        retryLimit: 5,
                        minInterval: TimeSpan.FromSeconds(1),
                        maxInterval: TimeSpan.FromSeconds(30),
                        intervalDelta: TimeSpan.FromSeconds(2)));

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
