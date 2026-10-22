using Seguradora.Domain.Apolices;
using Seguradora.Domain.Common;
using Seguradora.Domain.Sinistros;
using Xunit;

namespace Seguradora.Domain.Tests.Apolices;

public class ApoliceTests
{
    private static readonly DateTime Hoje = new(2026, 10, 15);

    [Fact]
    public void CriarComDadosValidosDeveRetornarApoliceAtiva()
    {
        var apolice = Apolice.Criar("Maria Silva", 50_000m, Hoje, Hoje.AddYears(1));

        Assert.NotEqual(Guid.Empty, apolice.Id);
        Assert.Equal(StatusApolice.Ativa, apolice.Status);
        Assert.Equal(50_000m, apolice.ValorCobertura);
    }

    [Fact]
    public void CriarComCoberturaZeroDeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Apolice.Criar("Maria Silva", 0m, Hoje, Hoje.AddYears(1)));
    }

    [Fact]
    public void CriarComFimAntesDoInicioDeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Apolice.Criar("Maria Silva", 50_000m, Hoje, Hoje.AddDays(-1)));
    }

    [Fact]
    public void CriarSemSeguradoDeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Apolice.Criar(" ", 50_000m, Hoje, Hoje.AddYears(1)));
    }

    private static Apolice NovaApoliceVigente() =>
    Apolice.Criar("Maria Silva", 50_000m, Hoje.AddMonths(-1), Hoje.AddYears(1));

    [Fact]
    public void AbrirSinistroValidoDeveRetornarSinistroAberto()
    {
        var apolice = NovaApoliceVigente();

        var sinistro = apolice.AbrirSinistro("Colisão traseira", 10_000m, Hoje.AddDays(-2), Hoje);

        Assert.Equal(apolice.Id, sinistro.ApoliceId);
        Assert.Equal(StatusSinistro.Aberto, sinistro.Status);
        Assert.Equal(10_000m, sinistro.ValorEstimado);
    }

    [Fact]
    public void AbrirSinistroEmApoliceCanceladaDeveLancarDomainException()
    {
        var apolice = NovaApoliceVigente();
        apolice.Cancelar();

        Assert.Throws<DomainException>(() =>
            apolice.AbrirSinistro("Colisão", 10_000m, Hoje.AddDays(-2), Hoje));
    }

    [Fact]
    public void AbrirSinistroEmApoliceVencidaDeveLancarDomainException()
    {
        var apolice = Apolice.Criar("Maria Silva", 50_000m, Hoje.AddYears(-2), Hoje.AddYears(-1));

        Assert.Throws<DomainException>(() =>
            apolice.AbrirSinistro("Colisão", 10_000m, Hoje.AddYears(-1).AddDays(-5), Hoje));
    }

    [Fact]
    public void AbrirSinistroAcimaDaCoberturaDeveLancarDomainException()
    {
        var apolice = NovaApoliceVigente();

        var ex = Assert.Throws<DomainException>(() =>
            apolice.AbrirSinistro("Colisão", 50_000.01m, Hoje.AddDays(-2), Hoje));

        Assert.Contains("cobertura", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AbrirSinistroComOcorrenciaAnteriorAVigenciaDeveLancarDomainException()
    {
        var apolice = NovaApoliceVigente();

        Assert.Throws<DomainException>(() =>
            apolice.AbrirSinistro("Colisão", 10_000m, Hoje.AddMonths(-2), Hoje));
    }

    [Fact]
    public void AbrirSinistroComOcorrenciaNoFuturoDeveLancarDomainException()
    {
        var apolice = NovaApoliceVigente();

        Assert.Throws<DomainException>(() =>
            apolice.AbrirSinistro("Colisão", 10_000m, Hoje.AddDays(1), Hoje));
    }
}