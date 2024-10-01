using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGFME.Domain.Entidades;

namespace SGFME.Infrastructure.Data.Mapping
{
    public class DispensacaoMapping : IEntityTypeConfiguration<Dispensacao>
    {
        public void Configure(EntityTypeBuilder<Dispensacao> builder)
        {
            // Define o nome da tabela
            builder.ToTable("Dispensacao");

            // Configura a chave primária
            builder.HasKey(d => d.id);

            // Configura o relacionamento com Paciente (muitos para um)
            builder.HasOne(d => d.paciente)
                .WithMany()
                .HasForeignKey(d => d.idPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            // Configura o relacionamento com Cid (muitos para um)
            builder.HasOne(d => d.cid)
                .WithMany()
                .HasForeignKey(d => d.idCid)
                .OnDelete(DeleteBehavior.Restrict);

            // Configura o campo de início do APAC
            builder.Property(d => d.inicioApac)
                .IsRequired();

            // Configura o campo de fim do APAC
            builder.Property(d => d.fimApac)
                .IsRequired();

            // Configura o campo de observação
            builder.Property(d => d.observacao)
                .HasMaxLength(500);

            // Configura o relacionamento com StatusProcesso (muitos para um)
            builder.HasOne(d => d.statusprocesso)
                .WithMany()
                .HasForeignKey(d => d.idStatusProcesso)
                .OnDelete(DeleteBehavior.Restrict);

            // Configura o relacionamento com TipoProcesso (muitos para um)
            builder.HasOne(d => d.tipoprocesso)
                .WithMany()
                .HasForeignKey(d => d.idTipoProcesso)
                .OnDelete(DeleteBehavior.Restrict);

            // Configura o relacionamento com Medicamentos (um para muitos)
            builder.HasMany(d => d.medicamento)
                .WithOne()
                .HasForeignKey(m => m.idDispensacao)
                .OnDelete(DeleteBehavior.Cascade);

            // Configura os campos opcionais de data de renovação e suspensão
            builder.Property(d => d.dataRenovacao)
                .IsRequired(false);

            builder.Property(d => d.dataSuspensao)
                .IsRequired(false);
        }
    }
}
