using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dev_PUC_SoSDog.Migrations
{
    /// <inheritdoc />
    public partial class AddNovosCamposOcorrencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Ocorrencias",
                newName: "Endereco");

            migrationBuilder.AlterColumn<float>(
                name: "Longitude",
                table: "Ocorrencias",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<float>(
                name: "Latitude",
                table: "Ocorrencias",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<string>(
                name: "Codigo_Cachorro",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cor_Pelagem",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Data_Ultima_Alimentacao",
                table: "Ocorrencias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Faixa_Etaria",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Porte",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Recebeu_Agua",
                table: "Ocorrencias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Recebeu_Comida",
                table: "Ocorrencias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Sexo",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sociabilidade",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codigo_Cachorro",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "Cor_Pelagem",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "Data_Ultima_Alimentacao",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "Faixa_Etaria",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "Porte",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "Recebeu_Agua",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "Recebeu_Comida",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "Sexo",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "Sociabilidade",
                table: "Ocorrencias");

            migrationBuilder.RenameColumn(
                name: "Endereco",
                table: "Ocorrencias",
                newName: "Status");

            migrationBuilder.AlterColumn<float>(
                name: "Longitude",
                table: "Ocorrencias",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Latitude",
                table: "Ocorrencias",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);
        }
    }
}
