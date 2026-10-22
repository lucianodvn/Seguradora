using NSubstitute;
using Seguradora.Application.Common;
using Seguradora.Application.Sinistros;
using Seguradora.Domain.Sinistros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Tests.Sinistros
{
    public class ListarSinistrosHandlerTests
    {
        private readonly ISinistroConsultas _consultas = Substitute.For<ISinistroConsultas>();
        private readonly ListarSinistrosHandler _handler;

        public ListarSinistrosHandlerTests()
        {
            _handler = new ListarSinistrosHandler(_consultas);
        }

        [Fact]
        public async Task FiltroValidoDeveConverterStatusERepassarAConsulta()
        {
            var apoliceId = Guid.NewGuid();
            var esperado = new PaginaResultado<SinistroDto>([], 2, 10, 0);
            _consultas.ListarAsync(Arg.Any<FiltroSinistros>(), Arg.Any<CancellationToken>()).Returns(esperado);

            var resultado = await _handler.HandleAsync(
                new ListarSinistrosQuery(apoliceId, "emanalise", null, null, 2, 10), CancellationToken.None);

            Assert.Same(esperado, resultado);
            await _consultas.Received(1).ListarAsync(
                Arg.Is<FiltroSinistros>(f =>
                    f.ApoliceId == apoliceId &&
                    f.Status == StatusSinistro.EmAnalise &&
                    f.Pagina == 2 &&
                    f.TamanhoPagina == 10),
                Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData(0, 20)]
        [InlineData(1, 0)]
        [InlineData(1, 101)]
        public async Task PaginacaoInvalidaDeveLancarEntradaInvalida(int pagina, int tamanho)
        {
            await Assert.ThrowsAsync<EntradaInvalidaException>(() => _handler.HandleAsync(
                new ListarSinistrosQuery(null, null, null, null, pagina, tamanho), CancellationToken.None));

            await _consultas.DidNotReceive().ListarAsync(Arg.Any<FiltroSinistros>(), Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData("Inexistente")]
        [InlineData("99")]
        public async Task StatusInvalidoDeveLancarEntradaInvalida(string status)
        {
            await Assert.ThrowsAsync<EntradaInvalidaException>(() => _handler.HandleAsync(
                new ListarSinistrosQuery(null, status, null, null), CancellationToken.None));
        }

        [Fact]
        public async Task IntervaloDeDatasInvertidoDeveLancarEntradaInvalida()
        {
            var hoje = new DateTime(2026, 10, 19, 0, 0, 0, DateTimeKind.Utc);

            await Assert.ThrowsAsync<EntradaInvalidaException>(() => _handler.HandleAsync(
                new ListarSinistrosQuery(null, null, hoje, hoje.AddDays(-1)), CancellationToken.None));
        }
    }
}
