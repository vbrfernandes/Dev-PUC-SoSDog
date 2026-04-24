using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dev_PUC_SoSDog.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    ID_Usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Foto_Perfil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.ID_Usuario);
                });

            migrationBuilder.CreateTable(
                name: "Ocorrencias",
                columns: table => new
                {
                    ID_Ocorrencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Foto_Animal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<float>(type: "real", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false),
                    Data_Registro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ID_Usuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ocorrencias", x => x.ID_Ocorrencia);
                    table.ForeignKey(
                        name: "FK_Ocorrencias_Usuarios_ID_Usuario",
                        column: x => x.ID_Usuario,
                        principalTable: "Usuarios",
                        principalColumn: "ID_Usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    ID_Comentario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data_hora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ID_Usuario = table.Column<int>(type: "int", nullable: false),
                    ID_Ocorrencia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.ID_Comentario);
                    table.ForeignKey(
                        name: "FK_Comentarios_Ocorrencias_ID_Ocorrencia",
                        column: x => x.ID_Ocorrencia,
                        principalTable: "Ocorrencias",
                        principalColumn: "ID_Ocorrencia",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comentarios_Usuarios_ID_Usuario",
                        column: x => x.ID_Usuario,
                        principalTable: "Usuarios",
                        principalColumn: "ID_Usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favoritos",
                columns: table => new
                {
                    ID_Favorito = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Usuario = table.Column<int>(type: "int", nullable: false),
                    ID_Ocorrencia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoritos", x => x.ID_Favorito);
                    table.ForeignKey(
                        name: "FK_Favoritos_Ocorrencias_ID_Ocorrencia",
                        column: x => x.ID_Ocorrencia,
                        principalTable: "Ocorrencias",
                        principalColumn: "ID_Ocorrencia",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Favoritos_Usuarios_ID_Usuario",
                        column: x => x.ID_Usuario,
                        principalTable: "Usuarios",
                        principalColumn: "ID_Usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_ID_Ocorrencia",
                table: "Comentarios",
                column: "ID_Ocorrencia");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_ID_Usuario",
                table: "Comentarios",
                column: "ID_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_ID_Ocorrencia",
                table: "Favoritos",
                column: "ID_Ocorrencia");

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_ID_Usuario",
                table: "Favoritos",
                column: "ID_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_ID_Usuario",
                table: "Ocorrencias",
                column: "ID_Usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "Favoritos");

            migrationBuilder.DropTable(
                name: "Ocorrencias");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
