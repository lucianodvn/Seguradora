using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Seguradora.Application.Common;
using Seguradora.Domain.Common;
using System.Diagnostics;

namespace Seguradora.Api.Middleware
{
    internal sealed partial class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetails) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title, detail) = exception switch
            {
                NaoEncontradoException => (StatusCodes.Status404NotFound, "Recurso não encontrado.", exception.Message),
                EntradaInvalidaException => (StatusCodes.Status400BadRequest, "Entrada inválida", exception.Message),
                DomainException => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada.", exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno", "Ocorreu um erro inesperado.")
            };

            if (status == StatusCodes.Status500InternalServerError)
                LogErro(logger, exception);
            else
                LogRejeitada(logger, exception.Message);

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };
            problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = status;
            return await problemDetails.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem,
                Exception = exception
            });
        }

        [LoggerMessage(Level = LogLevel.Warning, Message = "Requisição rejeitada: {Mensagem}")]
        private static partial void LogRejeitada(ILogger logger, string mensagem);

        [LoggerMessage(Level = LogLevel.Error, Message = "Erro não tratado")]
        private static partial void LogErro(ILogger logger, Exception exception);
    }
}
