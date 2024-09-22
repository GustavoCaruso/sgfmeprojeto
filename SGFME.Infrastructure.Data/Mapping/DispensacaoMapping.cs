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
                .WithMany(p => p.dispensacao)
                .HasForeignKey(d => d.idPaciente);


            // Configura o relacionamento com Paciente (muitos para um)
            builder.HasOne(d => d.cid)
                .WithMany()
                .HasForeignKey(d => d.idCid);
                

            // Configura o campo de início
            builder.Property(d => d.inicioApac)
                .IsRequired();


            // Configura o campo de fim
            builder.Property(d => d.fimApac)
                .IsRequired();
                

            // Configura o campo de observação
            builder.Property(d => d.observacao)
                .HasMaxLength(500)
                .HasColumnType("varchar(500)");

            // Configura o relacionamento com Medicamentos (um para muitos)
            builder.HasMany(d => d.dispensacaomedicamento)
                .WithOne()
                .HasForeignKey(m => m.idDispensacao);
               
        }
    }
}
