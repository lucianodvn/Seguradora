using Dapper;
using Seguradora.Application.Common;
using Seguradora.Application.Sinistros;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Infrastructure.Persistence.Consultas
{
    internal sealed class SinistroConsultas(SqlConnectionFactory conexoes) : ISinistroConsultas
    {
        public async Task<PaginaResultado<SinistroDto>> ListarAsync(FiltroSinistros filtro, CancellationToken cancellationToken)
        {
            var condicoes = new List<string>();
            var parametros = new DynamicParameters();

            if (filtro.ApoliceId is { } apoliceId)
            {
                condicoes.Add("ApoliceId = @ApoliceId");
                parametros.Add("ApoliceId", apoliceId, DbType.Guid);
            }

            if (filtro.Status is { } status)
            {
                condicoes.Add("Status = @Status");
                parametros.Add("Status", status.ToString(), DbType.String, size: 20);
            }
            if (filtro.AbertoDesde is { } desde)
            {
                condicoes.Add("DataAbertura >= @AbertoDesde");
                parametros.Add("AbertoDesde", desde, DbType.DateTime2);
            }
            if (filtro.AbertoAte is { } ate)
            {
                condicoes.Add("DataAbertura <= @AbertoAte");
                parametros.Add("AbertoAte", ate, DbType.DateTime2);
            }

            var filtroSql = condicoes.Count > 0 ? "WHERE " + string.Join(" AND ", condicoes) : string.Empty;

            parametros.Add("Offset", (long)(filtro.Pagina - 1) * filtro.TamanhoPagina, DbType.Int64);
            parametros.Add("Limite", filtro.TamanhoPagina, DbType.Int32);

            var sql = $"""
            SELECT COUNT(*) FROM Sinistros {filtroSql};

            SELECT Id, ApoliceId, Descricao, ValorEstimado, DataOcorrencia, DataAbertura, Status, MotivoNegativa
            FROM Sinistros
            {filtroSql}
            ORDER BY DataAbertura DESC, Id
            OFFSET @Offset ROWS FETCH NEXT @Limite ROWS ONLY;
            """;

            await using var conexao = conexoes.Criar();
            using var grade = await conexao.QueryMultipleAsync(
                new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));

            var total = await grade.ReadSingleAsync<int>();
            var linhas = await grade.ReadAsync<LinhaSinistro>();

            var itens = linhas
                .Select(l => new SinistroDto(
                    l.Id, l.ApoliceId, l.Descricao, l.ValorEstimado,
                    Utc(l.DataOcorrencia), Utc(l.DataAbertura), l.Status, l.MotivoNegativa))
                .ToList();

            return new PaginaResultado<SinistroDto>(itens, filtro.Pagina, filtro.TamanhoPagina, total);
        }

        private static DateTime Utc(DateTime valor) => DateTime.SpecifyKind(valor, DateTimeKind.Utc);

        internal sealed record LinhaSinistro(
            Guid Id, Guid ApoliceId, string Descricao, decimal ValorEstimado,
            DateTime DataOcorrencia, DateTime DataAbertura, string Status, string? MotivoNegativa);
    }
}
