using Microsoft.Extensions.DependencyInjection;
using Seguradora.Application.Abstractions;
using Seguradora.Application.Apolices;
using Seguradora.Application.Common;
using Seguradora.Application.Sinistros;
using Seguradora.Application.Sinistros.AbrirSinistro;
using Seguradora.Application.Sinistros.ObterSinistro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton(TimeProvider.System);

            services.AddScoped<ICommandHandler<CriarApoliceCommand, ApoliceDto>, CriarApoliceHandler>();
            services.AddScoped<ICommandHandler<AbrirSinistroCommand, SinistroDto>, AbrirSinistroHandler>();
            services.AddScoped<IQueryHandler<ObterSinistroQuery, SinistroDto>, ObterSinistroHandler>();
            services.AddScoped<IQueryHandler<ObterApoliceQuery, ApoliceDto>, ObterApoliceHandler>();
            services.AddScoped<IQueryHandler<ListarSinistrosQuery, PaginaResultado<SinistroDto>>, ListarSinistrosHandler>();

            return services;
        }
    }
}
