using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGFME.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecisaTrocarSenhaColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adiciona a coluna "precisaTrocarSenha" à tabela "Usuario"
            migrationBuilder.AddColumn<bool>(
                name: "precisaTrocarSenha",
                table: "Usuario",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove a coluna "precisaTrocarSenha" da tabela "Usuario"
            migrationBuilder.DropColumn(
                name: "precisaTrocarSenha",
                table: "Usuario");
        }
    }
}
