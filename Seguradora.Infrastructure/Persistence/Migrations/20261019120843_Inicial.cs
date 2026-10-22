using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seguradora.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apolices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Segurado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ValorCobertura = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VigenciaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VigenciaFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apolices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sinistros",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApoliceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ValorEstimado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataOcorrencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MotivoNegativa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sinistros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sinistros_Apolices_ApoliceId",
                        column: x => x.ApoliceId,
                        principalTable: "Apolices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sinistros_ApoliceId",
                table: "Sinistros",
                column: "ApoliceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sinistros");

            migrationBuilder.DropTable(
                name: "Apolices");
        }
    }
}
