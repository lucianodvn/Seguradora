using NSubstitute;
using Seguradora.Application.Apolices;
using Seguradora.Application.Common;
using Seguradora.Domain.Apolices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Tests.Apolices
{
    public class ObterApoliceHandlerTests
    {
        private static readonly DateTime Hoje = new(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);

        private readonly IApoliceRepository _apolices = Substitute.For<IApoliceRepository>();
        private readonly ObterApoliceHandler _handler;

        public ObterApoliceHandlerTests()
        {
            _handler = new ObterApoliceHandler(_apolices);
        }

        [Fact]
        public async Task ApoliceExistenteDeveRetornarDto()
        {
            var apolice = Apolice.Criar("Maria Silva", 50_000m, Hoje, Hoje.AddYears(1));
            _apolices.ObterPorIdAsync(apolice.Id, Arg.Any<CancellationToken>()).Returns(apolice);

            var resultado = await _handler.HandleAsync(new ObterApoliceQuery(apolice.Id), CancellationToken.None);

            Assert.Equal(apolice.Id, resultado.Id);
            Assert.Equal("Maria Silva", resultado.Segurado);
            Assert.Equal("Ativa", resultado.Status);
        }

        [Fact]
        public async Task ApoliceInexistenteDeveLancarNaoEncontradoException()
        {
            var id = Guid.NewGuid();
            _apolices.ObterPorIdAsync(id, Arg.Any<CancellationToken>()).Returns((Apolice?)null);

            await Assert.ThrowsAsync<NaoEncontradoException>(
                () => _handler.HandleAsync(new ObterApoliceQuery(id), CancellationToken.None));
        }
    }
}
