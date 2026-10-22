using System.ComponentModel.DataAnnotations;

namespace Seguradora.Api.Contracts.Auth
{
    public sealed record LoginRequest([Required] string Usuario, [Required] string Senha);
}
