using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Seguradora.Api.Contracts.Auth
{
    public sealed class TokenService(IOptions<JwtSettings> options, TimeProvider timeProvider)
    {
        private readonly JwtSettings _settings = options.Value;

        public (string Token, DateTimeOffset Expiracao) Gerar(string usuario)
        {
            var agora = timeProvider.GetUtcNow();
            var expiracao = agora.AddMinutes(_settings.ExpiracaoMinutos);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: agora.UtcDateTime,
                expires: expiracao.UtcDateTime,
                signingCredentials: credenciais);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiracao);
        }
    }
}
