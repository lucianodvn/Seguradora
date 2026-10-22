using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace Seguradora.Api.IntegrationTests
{
    public sealed class SinistrosApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        private async Task AutenticarAsync()
        {
            var resposta = await _client.PostAsJsonAsync("/api/auth/token",
                new { usuario = ApiFactory.Usuario, senha = ApiFactory.Senha });
            resposta.EnsureSuccessStatusCode();

            var json = await resposta.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(json);

            string? token = null;
            foreach (var propriedade in doc.RootElement.EnumerateObject())
            {
                if (propriedade.Value.ValueKind == System.Text.Json.JsonValueKind.String &&
                    propriedade.Name.Contains("token", StringComparison.OrdinalIgnoreCase))
                {
                    token = propriedade.Value.GetString();
                    break;
                }
            }

            Assert.False(string.IsNullOrWhiteSpace(token), $"Token não encontrado na resposta: {json}");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private sealed record TokenResposta(string Token);

        [Fact]
        public async Task ListarSemTokenRetorna401()
        {
            var resposta = await _client.GetAsync("/api/sinistros");

            Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        }

        [Fact]
        public async Task ObterApoliceInexistenteRetorna404ComProblemDetails()
        {
            await AutenticarAsync();

            var resposta = await _client.GetAsync($"/api/apolices/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Equal("application/problem+json", resposta.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task FluxoCompletoCriaApoliceAbreSinistroELista()
        {
            await AutenticarAsync();

            // 1. cria apólice (ajuste o corpo ao seu CriarApoliceCommand)
            var agora = DateTimeOffset.UtcNow;

            var criar = await _client.PostAsJsonAsync("/api/apolices", new
            {
                segurado = "Maria Silva",
                valorCobertura = 50000m,
                vigenciaInicio = agora.AddDays(-1),
                vigenciaFim = agora.AddYears(1)
            });
            Assert.True(criar.StatusCode == HttpStatusCode.Created, await criar.Content.ReadAsStringAsync());
            var apoliceUrl = criar.Headers.Location!;

            // 2. abre sinistro nessa apólice (ajuste rota/corpo)
            var abrir = await _client.PostAsJsonAsync($"{apoliceUrl}/sinistros", new
            {
                descricao = "Colisão traseira em estacionamento",
                valorEstimado = 3500m,
                dataOcorrencia = agora.AddHours(-1)
            });
            Assert.True(abrir.StatusCode == HttpStatusCode.Created, await abrir.Content.ReadAsStringAsync());

            // 3. lista (Dapper) e confere que o sinistro aparece
            var lista = await _client.GetAsync("/api/sinistros?pagina=1&tamanhoPagina=10");
            Assert.Equal(HttpStatusCode.OK, lista.StatusCode);
            var corpo = await lista.Content.ReadAsStringAsync();
            Assert.Contains("Aberto", corpo);   // ajuste ao status inicial real
        }
    }
}
