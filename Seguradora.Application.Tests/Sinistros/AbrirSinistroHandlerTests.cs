using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Seguradora.Application.Abstractions;
using Seguradora.Application.Common;
using Seguradora.Application.Eventos;
using Seguradora.Application.Sinistros;
using Seguradora.Application.Sinistros.AbrirSinistro;
using Seguradora.Domain.Apolices;
using Seguradora.Domain.Common;
using Seguradora.Domain.Sinistros;
using Xunit;

namespace Seguradora.Application.Tests.Sinistros;

public class AbrirSinistroHandlerTests
{
    private static readonly DateTime Hoje = new(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);
    private readonly IApoliceRepository _apolices = Substitute.For<IApoliceRepository>();
    private readonly ISinistroRepository _sinistros = Substitute.For<ISinistroRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPublicadorEventos _publicador = Substitute.For<IPublicadorEventos>();
    private readonly AbrirSinistroHandler _handler;

    public AbrirSinistroHandlerTests()
    {
        var relogio = new FakeTimeProvider(new DateTimeOffset(Hoje));
        _handler = new AbrirSinistroHandler(_apolices, _sinistros, _unitOfWork, relogio, _publicador);
    }

    private static Apolice NovaApolice() =>
        Apolice.Criar("Maria Silva", 50_000m, Hoje.AddMonths(-1), Hoje.AddYears(1));

    [Fact]
    public async Task ApoliceExistenteDevePersistirSinistroESalvar()
    {
        var apolice = NovaApolice();
        _apolices.ObterPorIdAsync(apolice.Id, Arg.Any<CancellationToken>()).Returns(apolice);
        var comando = new AbrirSinistroCommand(apolice.Id, "Colisão traseira", 10_000m, Hoje.AddDays(-2));

        var resultado = await _handler.HandleAsync(comando, CancellationToken.None);

        Assert.Equal("Aberto", resultado.Status);
        Assert.Equal(apolice.Id, resultado.ApoliceId);
        await _sinistros.Received(1).AdicionarAsync(Arg.Any<Sinistro>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SalvarAsync(Arg.Any<CancellationToken>());
        await _publicador.Received(1).PublicarAsync(Arg.Is<SinistroAberto>(e => e.ApoliceId == apolice.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ApoliceInexistenteDeveLancarNaoEncontradoESemSalvar()
    {
        var id = Guid.NewGuid();
        _apolices.ObterPorIdAsync(id, Arg.Any<CancellationToken>()).Returns((Apolice?)null);
        var comando = new AbrirSinistroCommand(id, "Colisão", 10_000m, Hoje.AddDays(-2));

        await Assert.ThrowsAsync<NaoEncontradoException>(
            () => _handler.HandleAsync(comando, CancellationToken.None));

        await _unitOfWork.DidNotReceive().SalvarAsync(Arg.Any<CancellationToken>());

        await _publicador.DidNotReceive().PublicarAsync(Arg.Any<SinistroAberto>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ValorAcimaDaCoberturaDeveLancarDomainExceptionESemSalvar()
    {
        var apolice = NovaApolice();
        _apolices.ObterPorIdAsync(apolice.Id, Arg.Any<CancellationToken>()).Returns(apolice);
        var comando = new AbrirSinistroCommand(apolice.Id, "Colisão", 60_000m, Hoje.AddDays(-2));

        await Assert.ThrowsAsync<DomainException>(
            () => _handler.HandleAsync(comando, CancellationToken.None));

        await _sinistros.DidNotReceive().AdicionarAsync(Arg.Any<Sinistro>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SalvarAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DevePublicarEventoAntesDeSalvarParaIrNaMesmaTransacao()
    {
        var apolice = NovaApolice();
        _apolices.ObterPorIdAsync(apolice.Id, Arg.Any<CancellationToken>()).Returns(apolice);
        var comando = new AbrirSinistroCommand(apolice.Id, "Colisão traseira", 10_000m, Hoje.AddDays(-2));

        await _handler.HandleAsync(comando, CancellationToken.None);

        Received.InOrder(() =>
        {
            _ = _publicador.PublicarAsync(Arg.Any<SinistroAberto>(), Arg.Any<CancellationToken>());
            _ = _unitOfWork.SalvarAsync(Arg.Any<CancellationToken>());
        });
    }
}