using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGFME.Infrastructure.Data.Migrations
{
    public partial class AddRelacaoPacienteRepresentante : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Verifique se os índices já existem antes de tentar criá-los
            // Remove a tentativa de recriar índices que já estão presentes
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remover apenas os índices, caso eles existam
            if (migrationBuilder.Sql("SELECT 1 FROM sys.indexes WHERE name = 'IX_PacienteRepresentante_idPaciente'") != null)
            {
                migrationBuilder.DropIndex(
                    name: "IX_PacienteRepresentante_idPaciente",
                    table: "PacienteRepresentante");
            }

            if (migrationBuilder.Sql("SELECT 1 FROM sys.indexes WHERE name = 'IX_PacienteRepresentante_idRepresentante'") != null)
            {
                migrationBuilder.DropIndex(
                    name: "IX_PacienteRepresentante_idRepresentante",
                    table: "PacienteRepresentante");
            }
        }
    }
}
