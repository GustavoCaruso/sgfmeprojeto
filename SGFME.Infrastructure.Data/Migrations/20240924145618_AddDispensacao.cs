using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGFME.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDispensacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispensacaoMedicamentos_Dispensacao_idMedicamento",
                table: "DispensacaoMedicamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_DispensacaoMedicamentos_Medicamento_idMedicamento",
                table: "DispensacaoMedicamentos");

            migrationBuilder.RenameColumn(
                name: "idMedicamento",
                table: "DispensacaoMedicamentos",
                newName: "IdMedicamento");

            migrationBuilder.RenameIndex(
                name: "IX_DispensacaoMedicamentos_idMedicamento",
                table: "DispensacaoMedicamentos",
                newName: "IX_DispensacaoMedicamentos_IdMedicamento");

            migrationBuilder.AlterColumn<string>(
                name: "observacao",
                table: "Dispensacao",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_DispensacaoMedicamentos_idDispensacao",
                table: "DispensacaoMedicamentos",
                column: "idDispensacao");

            migrationBuilder.AddForeignKey(
                name: "FK_DispensacaoMedicamentos_Dispensacao_idDispensacao",
                table: "DispensacaoMedicamentos",
                column: "idDispensacao",
                principalTable: "Dispensacao",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DispensacaoMedicamentos_Medicamento_IdMedicamento",
                table: "DispensacaoMedicamentos",
                column: "IdMedicamento",
                principalTable: "Medicamento",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispensacaoMedicamentos_Dispensacao_idDispensacao",
                table: "DispensacaoMedicamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_DispensacaoMedicamentos_Medicamento_IdMedicamento",
                table: "DispensacaoMedicamentos");

            migrationBuilder.DropIndex(
                name: "IX_DispensacaoMedicamentos_idDispensacao",
                table: "DispensacaoMedicamentos");

            migrationBuilder.RenameColumn(
                name: "IdMedicamento",
                table: "DispensacaoMedicamentos",
                newName: "idMedicamento");

            migrationBuilder.RenameIndex(
                name: "IX_DispensacaoMedicamentos_IdMedicamento",
                table: "DispensacaoMedicamentos",
                newName: "IX_DispensacaoMedicamentos_idMedicamento");

            migrationBuilder.AlterColumn<string>(
                name: "observacao",
                table: "Dispensacao",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DispensacaoMedicamentos_Dispensacao_idMedicamento",
                table: "DispensacaoMedicamentos",
                column: "idMedicamento",
                principalTable: "Dispensacao",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DispensacaoMedicamentos_Medicamento_idMedicamento",
                table: "DispensacaoMedicamentos",
                column: "idMedicamento",
                principalTable: "Medicamento",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
