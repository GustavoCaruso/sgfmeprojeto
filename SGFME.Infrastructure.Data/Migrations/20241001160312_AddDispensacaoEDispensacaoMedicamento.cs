using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGFME.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDispensacaoEDispensacaoMedicamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispensacao_Cid_idCid",
                table: "Dispensacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispensacao_Paciente_idPaciente",
                table: "Dispensacao");

            migrationBuilder.DropForeignKey(
                name: "FK_DispensacaoMedicamentos_Medicamento_IdMedicamento",
                table: "DispensacaoMedicamentos");

            migrationBuilder.RenameColumn(
                name: "IdMedicamento",
                table: "DispensacaoMedicamentos",
                newName: "idMedicamento");

            migrationBuilder.RenameColumn(
                name: "recebido",
                table: "DispensacaoMedicamentos",
                newName: "receita");

            migrationBuilder.RenameIndex(
                name: "IX_DispensacaoMedicamentos_IdMedicamento",
                table: "DispensacaoMedicamentos",
                newName: "IX_DispensacaoMedicamentos_idMedicamento");

            migrationBuilder.AddColumn<DateTime>(
                name: "dataEntrega",
                table: "DispensacaoMedicamentos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "medicamentoChegou",
                table: "DispensacaoMedicamentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "medicamentoEntregue",
                table: "DispensacaoMedicamentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "dataRenovacao",
                table: "Dispensacao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "dataSuspensao",
                table: "Dispensacao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "idStatusProcesso",
                table: "Dispensacao",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "idTipoProcesso",
                table: "Dispensacao",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Dispensacao_idStatusProcesso",
                table: "Dispensacao",
                column: "idStatusProcesso");

            migrationBuilder.CreateIndex(
                name: "IX_Dispensacao_idTipoProcesso",
                table: "Dispensacao",
                column: "idTipoProcesso");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispensacao_Cid_idCid",
                table: "Dispensacao",
                column: "idCid",
                principalTable: "Cid",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispensacao_Paciente_idPaciente",
                table: "Dispensacao",
                column: "idPaciente",
                principalTable: "Paciente",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispensacao_statusprocesso_idStatusProcesso",
                table: "Dispensacao",
                column: "idStatusProcesso",
                principalTable: "statusprocesso",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispensacao_tipoprocesso_idTipoProcesso",
                table: "Dispensacao",
                column: "idTipoProcesso",
                principalTable: "tipoprocesso",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DispensacaoMedicamentos_Medicamento_idMedicamento",
                table: "DispensacaoMedicamentos",
                column: "idMedicamento",
                principalTable: "Medicamento",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispensacao_Cid_idCid",
                table: "Dispensacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispensacao_Paciente_idPaciente",
                table: "Dispensacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispensacao_statusprocesso_idStatusProcesso",
                table: "Dispensacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispensacao_tipoprocesso_idTipoProcesso",
                table: "Dispensacao");

            migrationBuilder.DropForeignKey(
                name: "FK_DispensacaoMedicamentos_Medicamento_idMedicamento",
                table: "DispensacaoMedicamentos");

            migrationBuilder.DropIndex(
                name: "IX_Dispensacao_idStatusProcesso",
                table: "Dispensacao");

            migrationBuilder.DropIndex(
                name: "IX_Dispensacao_idTipoProcesso",
                table: "Dispensacao");

            migrationBuilder.DropColumn(
                name: "dataEntrega",
                table: "DispensacaoMedicamentos");

            migrationBuilder.DropColumn(
                name: "medicamentoChegou",
                table: "DispensacaoMedicamentos");

            migrationBuilder.DropColumn(
                name: "medicamentoEntregue",
                table: "DispensacaoMedicamentos");

            migrationBuilder.DropColumn(
                name: "dataRenovacao",
                table: "Dispensacao");

            migrationBuilder.DropColumn(
                name: "dataSuspensao",
                table: "Dispensacao");

            migrationBuilder.DropColumn(
                name: "idStatusProcesso",
                table: "Dispensacao");

            migrationBuilder.DropColumn(
                name: "idTipoProcesso",
                table: "Dispensacao");

            migrationBuilder.RenameColumn(
                name: "idMedicamento",
                table: "DispensacaoMedicamentos",
                newName: "IdMedicamento");

            migrationBuilder.RenameColumn(
                name: "receita",
                table: "DispensacaoMedicamentos",
                newName: "recebido");

            migrationBuilder.RenameIndex(
                name: "IX_DispensacaoMedicamentos_idMedicamento",
                table: "DispensacaoMedicamentos",
                newName: "IX_DispensacaoMedicamentos_IdMedicamento");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispensacao_Cid_idCid",
                table: "Dispensacao",
                column: "idCid",
                principalTable: "Cid",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispensacao_Paciente_idPaciente",
                table: "Dispensacao",
                column: "idPaciente",
                principalTable: "Paciente",
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
    }
}
