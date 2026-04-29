using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dev_PUC_SoSDog.Migrations
{
    /// <inheritdoc />
    public partial class AddUltimoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nome_Usuario_Ultima_Acao",
                table: "Ocorrencias",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nome_Usuario_Ultima_Acao",
                table: "Ocorrencias");
        }
    }
}
