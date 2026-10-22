using System.ComponentModel.DataAnnotations;

namespace Seguradora.Api.Contracts.Auth
{
    public sealed class JwtSettings
    {
        public const string Secao = "Jwt";
        [Required, MinLength(32)] public string Key { get; set; } = "";
        [Required] public string Issuer { get; set; } = "";
        [Required] public string Audience { get; set; } = "";
        [Range(1, 1440)] public int ExpiracaoMinutos { get; set; } = 60;
    }
}
