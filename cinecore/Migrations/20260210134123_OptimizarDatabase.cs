using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinecore.Migrations
{
    /// <inheritdoc />
    public partial class OptimizarDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCadastro",
                table: "Usuarios");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecoFinal",
                table: "Sessoes",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecoBase",
                table: "Sessoes",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "ProdutosAlimento",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "ProdutosAlimento",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorTotal",
                table: "PedidosAlimento",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorDesconto",
                table: "PedidosAlimento",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxaCancelamento",
                table: "PedidosAlimento",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "ItensPedidoAlimento",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxaReserva",
                table: "Ingressos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "Ingressos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "IngressoMeia_Preco",
                table: "Ingressos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CPF",
                table: "Usuarios",
                column: "CPF",
                unique: true,
                filter: "[CPF] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessoes_DataHorario",
                table: "Sessoes",
                column: "DataHorario");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosAlimento_DataPedido",
                table: "PedidosAlimento",
                column: "DataPedido");

            migrationBuilder.CreateIndex(
                name: "IX_Ingressos_DataCompra",
                table: "Ingressos",
                column: "DataCompra");

            migrationBuilder.CreateIndex(
                name: "IX_AlugueisSala_Inicio_Fim",
                table: "AlugueisSala",
                columns: new[] { "Inicio", "Fim" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_CPF",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Sessoes_DataHorario",
                table: "Sessoes");

            migrationBuilder.DropIndex(
                name: "IX_PedidosAlimento_DataPedido",
                table: "PedidosAlimento");

            migrationBuilder.DropIndex(
                name: "IX_Ingressos_DataCompra",
                table: "Ingressos");

            migrationBuilder.DropIndex(
                name: "IX_AlugueisSala_Inicio_Fim",
                table: "AlugueisSala");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCadastro",
                table: "Usuarios",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<float>(
                name: "PrecoFinal",
                table: "Sessoes",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<float>(
                name: "PrecoBase",
                table: "Sessoes",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<float>(
                name: "Preco",
                table: "ProdutosAlimento",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "ProdutosAlimento",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<float>(
                name: "ValorTotal",
                table: "PedidosAlimento",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<float>(
                name: "ValorDesconto",
                table: "PedidosAlimento",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<float>(
                name: "TaxaCancelamento",
                table: "PedidosAlimento",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<float>(
                name: "Preco",
                table: "ItensPedidoAlimento",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<float>(
                name: "TaxaReserva",
                table: "Ingressos",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<float>(
                name: "Preco",
                table: "Ingressos",
                type: "real",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "IngressoMeia_Preco",
                table: "Ingressos",
                type: "real",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
