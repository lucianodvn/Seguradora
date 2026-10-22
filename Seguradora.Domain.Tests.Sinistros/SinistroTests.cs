using Seguradora.Domain.Apolices;
using Seguradora.Domain.Common;
using Seguradora.Domain.Sinistros;
using Xunit;

namespace Seguradora.Domain.Tests.Sinistros;

public class SinistroTests
{
    private static readonly DateTime Hoje = new(2026, 10, 15);

    private static Sinistro NovoSinistro()
    {
        var apolice = Apolice.Criar("Maria Silva", 50_000m, Hoje.AddMonths(-1), Hoje.AddYears(1));
        return apolice.AbrirSinistro("Colisão traseira", 10_000m, Hoje.AddDays(-2), Hoje);
    }

    [Fact]
    public void IniciarAnaliseDeveMudarParaEmAnalise()
    {
        var sinistro = NovoSinistro();

        sinistro.IniciarAnalise();

        Assert.Equal(StatusSinistro.EmAnalise, sinistro.Status);
    }

    [Fact]
    public void IniciarAnaliseDuasVezesDeveLancarDomainException()
    {
        var sinistro = NovoSinistro();
        sinistro.IniciarAnalise();

        Assert.Throws<DomainException>(() => sinistro.IniciarAnalise());
    }

    [Fact]
    public void AprovarSemAnaliseDeveLancarDomainException()
    {
        var sinistro = NovoSinistro();

        Assert.Throws<DomainException>(() => sinistro.Aprovar());
    }

    [Fact]
    public void AprovarAposAnaliseDeveMudarParaAprovado()
    {
        var sinistro = NovoSinistro();
        sinistro.IniciarAnalise();

        sinistro.Aprovar();

        Assert.Equal(StatusSinistro.Aprovado, sinistro.Status);
    }

    [Fact]
    public void NegarSemMotivoDeveLancarDomainException()
    {
        var sinistro = NovoSinistro();
        sinistro.IniciarAnalise();

        Assert.Throws<DomainException>(() => sinistro.Negar(" "));
    }

    [Fact]
    public void NegarAposAnaliseDeveMudarParaNegadoEGuardarMotivo()
    {
        var sinistro = NovoSinistro();
        sinistro.IniciarAnalise();

        sinistro.Negar("Fora da cobertura contratada");

        Assert.Equal(StatusSinistro.Negado, sinistro.Status);
        Assert.Equal("Fora da cobertura contratada", sinistro.MotivoNegativa);
    }

    [Fact]
    public void AprovarSinistroJaNegadoDeveLancarDomainException()
    {
        var sinistro = NovoSinistro();
        sinistro.IniciarAnalise();
        sinistro.Negar("Fora da cobertura");

        Assert.Throws<DomainException>(() => sinistro.Aprovar());
    }
}