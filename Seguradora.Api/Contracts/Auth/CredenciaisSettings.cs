using System.ComponentModel.DataAnnotations;

namespace Seguradora.Api.Contracts.Auth
{
    public sealed class CredenciaisSettings
    {
        public const string Secao = "Auth";
        [Required] public string Usuario { get; set; } = "";
        [Required] public string Senha { get; set; } = "";
    }
}
