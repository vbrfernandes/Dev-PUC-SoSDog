using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dev_PUC_SoSDog.Migrations
{
    /// <inheritdoc />
    public partial class SepararAguaComida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data_Ultima_Alimentacao",
                table: "Ocorrencias",
                newName: "Data_Ultima_Comida");

            migrationBuilder.AddColumn<DateTime>(
                name: "Data_Ultima_Agua",
                table: "Ocorrencias",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data_Ultima_Agua",
                table: "Ocorrencias");

            migrationBuilder.RenameColumn(
                name: "Data_Ultima_Comida",
                table: "Ocorrencias",
                newName: "Data_Ultima_Alimentacao");
        }
    }
}
