using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGFME.Domain.Entidades;

namespace SGFME.Infrastructure.Data.Mapping
{
    internal class PacienteRepresentanteMapping : IEntityTypeConfiguration<PacienteRepresentante>
    {
        public void Configure(EntityTypeBuilder<PacienteRepresentante> builder)
        {
            // Definindo o nome da tabela
            builder.ToTable("PacienteRepresentante");

            // Configurando a chave primária (Id já vem da BaseEntity)
            builder.HasKey(pr => pr.id);

            // Configurando o relacionamento com Paciente
            builder.HasOne(pr => pr.paciente)
                .WithMany(p => p.pacienterepresentante)
                .HasForeignKey(pr => pr.idPaciente)
                .OnDelete(DeleteBehavior.SetNull);

            // Configurando o relacionamento com Representante
            builder.HasOne(pr => pr.representante)
                .WithMany(r => r.pacienterepresentante)
                .HasForeignKey(pr => pr.idRepresentante)
                .OnDelete(DeleteBehavior.SetNull);

            // Propriedades adicionais (caso necessário, como DataRelacao ou outras)
        }
    }
}
