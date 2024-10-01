using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGFME.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDispensacaoEDispensacaoMedicamentoSingular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispensacaoMedicamentos_Dispensacao_idDispensacao",
                table: "DispensacaoMedicamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_DispensacaoMedicamentos_Medicamento_idMedicamento",
                table: "DispensacaoMedicamentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DispensacaoMedicamentos",
                table: "DispensacaoMedicamentos");

            migrationBuilder.RenameTable(
                name: "DispensacaoMedicamentos",
                newName: "DispensacaoMedicamento");

            migrationBuilder.RenameIndex(
                name: "IX_DispensacaoMedicamentos_idMedicamento",
                table: "DispensacaoMedicamento",
                newName: "IX_DispensacaoMedicamento_idMedicamento");

            migrationBuilder.RenameIndex(
                name: "IX_DispensacaoMedicamentos_idDispensacao",
                table: "DispensacaoMedicamento",
                newName: "IX_DispensacaoMedicamento_idDispensacao");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DispensacaoMedicamento",
                table: "DispensacaoMedicamento",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_DispensacaoMedicamento_Dispensacao_idDispensacao",
                table: "DispensacaoMedicamento",
                column: "idDispensacao",
                principalTable: "Dispensacao",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DispensacaoMedicamento_Medicamento_idMedicamento",
                table: "DispensacaoMedicamento",
                column: "idMedicamento",
                principalTable: "Medicamento",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispensacaoMedicamento_Dispensacao_idDispensacao",
                table: "DispensacaoMedicamento");

            migrationBuilder.DropForeignKey(
                name: "FK_DispensacaoMedicamento_Medicamento_idMedicamento",
                table: "DispensacaoMedicamento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DispensacaoMedicamento",
                table: "DispensacaoMedicamento");

            migrationBuilder.RenameTable(
                name: "DispensacaoMedicamento",
                newName: "DispensacaoMedicamentos");

            migrationBuilder.RenameIndex(
                name: "IX_DispensacaoMedicamento_idMedicamento",
                table: "DispensacaoMedicamentos",
                newName: "IX_DispensacaoMedicamentos_idMedicamento");

            migrationBuilder.RenameIndex(
                name: "IX_DispensacaoMedicamento_idDispensacao",
                table: "DispensacaoMedicamentos",
                newName: "IX_DispensacaoMedicamentos_idDispensacao");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DispensacaoMedicamentos",
                table: "DispensacaoMedicamentos",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_DispensacaoMedicamentos_Dispensacao_idDispensacao",
                table: "DispensacaoMedicamentos",
                column: "idDispensacao",
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
