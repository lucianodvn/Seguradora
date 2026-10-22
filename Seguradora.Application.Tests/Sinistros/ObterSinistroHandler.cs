using NSubstitute;
using Seguradora.Application.Abstractions;
using Seguradora.Application.Common;
using Seguradora.Application.Sinistros.AbrirSinistro;
using Seguradora.Application.Sinistros.ObterSinistro;
using Seguradora.Domain.Apolices;
using Seguradora.Domain.Sinistros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Tests.Sinistros
{
    public class ObterSinistroHandlerTests
    {
        private static readonly DateTime Hoje = new(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);

        private readonly ISinistroRepository _sinistros = Substitute.For<ISinistroRepository>();
        private readonly ObterSinistroHandler _handler;

        public ObterSinistroHandlerTests()
        {
            _handler = new ObterSinistroHandler(_sinistros);
        }

        private static Sinistro NovoSinistro()
        {
            var apolice = Apolice.Criar("Maria Silva", 50_000m, Hoje.AddMonths(-1), Hoje.AddYears(1));
            return apolice.AbrirSinistro("Colisão traseira", 10_000m, Hoje.AddDays(-2), Hoje);
        }

        [Fact]
        public async Task SinistroExistenteDeveRetornarDto()
        {
            var sinistro = NovoSinistro();
            _sinistros.ObterPorIdAsync(sinistro.Id, Arg.Any<CancellationToken>()).Returns(sinistro);

            var resultado = await _handler.HandleAsync(
                new ObterSinistroQuery(sinistro.Id), CancellationToken.None);

            Assert.Equal(sinistro.Id, resultado.Id);
            Assert.Equal(sinistro.ApoliceId, resultado.ApoliceId);
            Assert.Equal("Colisão traseira", resultado.Descricao);
            Assert.Equal(10_000m, resultado.ValorEstimado);
            Assert.Equal("Aberto", resultado.Status);
            Assert.Null(resultado.MotivoNegativa);
        }

        [Fact]
        public async Task SinistroNegadoDeveRetornarStatusEMotivoNoDto()
        {
            var sinistro = NovoSinistro();
            sinistro.IniciarAnalise();
            sinistro.Negar("Fora da cobertura contratada");
            _sinistros.ObterPorIdAsync(sinistro.Id, Arg.Any<CancellationToken>()).Returns(sinistro);

            var resultado = await _handler.HandleAsync(
                new ObterSinistroQuery(sinistro.Id), CancellationToken.None);

            Assert.Equal("Negado", resultado.Status);
            Assert.Equal("Fora da cobertura contratada", resultado.MotivoNegativa);
        }

        [Fact]
        public async Task SinistroInexistenteDeveLancarNaoEncontradoException()
        {
            var id = Guid.NewGuid();
            _sinistros.ObterPorIdAsync(id, Arg.Any<CancellationToken>()).Returns((Sinistro?)null);

            var ex = await Assert.ThrowsAsync<NaoEncontradoException>(
                () => _handler.HandleAsync(new ObterSinistroQuery(id), CancellationToken.None));

            Assert.Contains(id.ToString(), ex.Message, StringComparison.Ordinal);
        }
    }
}
