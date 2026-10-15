using NetArchTest.Rules;

using System.Reflection;


namespace Seguradora.Architecture.Tests
{
    public class ArquiteturaTests
    {
        private static readonly Assembly Domain = typeof(Seguradora.Domain.AssemblyReference).Assembly;
        private static readonly Assembly Application = typeof(Seguradora.Application.AssemblyReference).Assembly;
        private static readonly Assembly Infrastructure = typeof(Seguradora.Infrastructure.AssemblyReference).Assembly;
        private static readonly Assembly Api = typeof(Seguradora.Api.AssemblyReference).Assembly;

        [Fact]
        public void DomainNaoDependeDeNinguem()
        {
            var result = Types.InAssembly(Domain)
            .ShouldNot().HaveDependencyOnAny(
                "Seguradora.Application",
                "Seguradora.Infrastructure",
                "Seguradora.Api",
                "Microsoft.EntityFrameworkCore")
            .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void ApplicationNaoDependeDeInfrastructureNemApiNemEF()
        {
            var result = Types.InAssembly(Application)
                .ShouldNot().HaveDependencyOnAny(
                    "Seguradora.Infrastructure",
                    "Seguradora.Api",
                    "Microsoft.EntityFrameworkCore")
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void InfrastructureNaoDependeDaApi()
        {
            var result = Types.InAssembly(Infrastructure)
                .ShouldNot().HaveDependencyOn("Seguradora.Api")
                .GetResult();

            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void ControllersNaoConhecemAInfrastructure()
        {
            var result = Types.InAssembly(Api)
                .That().HaveNameEndingWith("Controller")
                .ShouldNot().HaveDependencyOn("Seguradora.Infrastructure")
                .GetResult();

            Assert.True(result.IsSuccessful);
        }
    }
}
