using NSubstitute;
using Seguradora.Application.Abstractions;
using Seguradora.Application.Apolices;
using Seguradora.Application.Sinistros.AbrirSinistro;
using Seguradora.Domain.Apolices;
using Seguradora.Domain.Sinistros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Time.Testing;
using Xunit;
using Seguradora.Domain.Common;

namespace Seguradora.Application.Tests.Apolices
{
    public class CriarApoliceHandlerTests
    {
        private static readonly DateTime Hoje = new(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);
        private readonly IApoliceRepository _apolices = Substitute.For<IApoliceRepository>();
        private readonly ISinistroRepository _sinistros = Substitute.For<ISinistroRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly CriarApoliceHandler _handler;

        public CriarApoliceHandlerTests()
        {
            _handler = new CriarApoliceHandler(_apolices, _unitOfWork);
        }

        [Fact]
        public async Task CriarApoliceDevePersistirESalvar()
        {
            var comando = new CriarApoliceCommand("João Silva", 100_000m, Hoje, Hoje.AddYears(1));
            var resultado = await _handler.HandleAsync(comando, CancellationToken.None);
            Assert.Equal("João Silva", resultado.Segurado);
            Assert.Equal(100_000m, resultado.ValorCobertura);
            Assert.Equal("Ativa", resultado.Status);
            await _apolices.Received(1).AdicionarAsync(Arg.Is<Apolice>(a => a.Segurado == "João Silva" && a.ValorCobertura == 100_000m), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SalvarAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CriarApoliceInexistenteDeveLancarNaoEncontradoESemSalvar()
        {
            var comando = new CriarApoliceCommand("João Silva", 0m, Hoje, Hoje.AddYears(1));
            await Assert.ThrowsAsync<DomainException>(
            () => _handler.HandleAsync(comando, CancellationToken.None));

            await _apolices.DidNotReceive().AdicionarAsync(Arg.Any<Apolice>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().SalvarAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CriarApoliceComVigenciaInvalidaDeveLancarDomainExceptionESemSalvar()
        {
            var comando = new CriarApoliceCommand("João Silva", 100_000m, Hoje.AddYears(1), Hoje);
            await Assert.ThrowsAsync<DomainException>(
                () => _handler.HandleAsync(comando, CancellationToken.None));
            await _apolices.DidNotReceive().AdicionarAsync(Arg.Any<Apolice>(), Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().SalvarAsync(Arg.Any<CancellationToken>());
        }
    }
}
