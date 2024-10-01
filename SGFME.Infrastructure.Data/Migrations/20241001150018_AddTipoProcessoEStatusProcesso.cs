using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGFME.Infrastructure.Data.Migrations
{
    public partial class AddTipoProcessoEStatusProcesso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Criação da tabela StatusProcesso
            migrationBuilder.CreateTable(
                name: "statusprocesso",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statusprocesso", x => x.id);
                });

            // Criação da tabela TipoProcesso
            migrationBuilder.CreateTable(
                name: "tipoprocesso",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipoprocesso", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remoção da tabela StatusProcesso
            migrationBuilder.DropTable(
                name: "statusprocesso");

            // Remoção da tabela TipoProcesso
            migrationBuilder.DropTable(
                name: "tipoprocesso");
        }
    }
}
