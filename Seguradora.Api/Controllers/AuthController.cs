using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Seguradora.Api.Contracts.Auth;
using System.Security.Cryptography;
using System.Text;

namespace Seguradora.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/auth")]
    public sealed class AuthController(TokenService tokens, IOptions<CredenciaisSettings> credenciais) : ControllerBase
    {
        [HttpPost("token")]
        [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
        public ActionResult<TokenResponse> Token(LoginRequest request)
        {
            var esperado = credenciais.Value;

            // '&' (e não '&&') para não revelar, pelo tempo de resposta, qual campo errou
            var valido = Iguais(request.Usuario, esperado.Usuario) & Iguais(request.Senha, esperado.Senha);
            if (!valido)
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Credenciais inválidas");

            var (token, expiraEm) = tokens.Gerar(request.Usuario);
            return Ok(new TokenResponse(token, expiraEm));
        }

        private static bool Iguais(string a, string b) =>
            CryptographicOperations.FixedTimeEquals(
                SHA256.HashData(Encoding.UTF8.GetBytes(a)),
                SHA256.HashData(Encoding.UTF8.GetBytes(b)));
    }
}
