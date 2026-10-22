namespace Seguradora.Api.Contracts.Auth
{
    public sealed record TokenResponse(string AccessToken, DateTimeOffset ExpiraEm, string TipoToken = "Bearer");
}
