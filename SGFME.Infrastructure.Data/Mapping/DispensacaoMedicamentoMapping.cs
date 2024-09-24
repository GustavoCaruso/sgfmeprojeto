using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGFME.Domain.Entidades;

namespace SGFME.Infrastructure.Data.Mapping
{
    public class DispensacaoMedicamentoMapping : IEntityTypeConfiguration<DispensacaoMedicamento>
    {
        public void Configure(EntityTypeBuilder<DispensacaoMedicamento> builder)
        {
            // Define o nome da tabela
            builder.ToTable("DispensacaoMedicamentos");

            // Configura a chave primária
            builder.HasKey(dm => dm.id);

            // Configura o relacionamento com Medicamento (muitos para um)
            builder.HasOne(dm => dm.Medicamento)
                .WithMany()
                .HasForeignKey(dm => dm.IdMedicamento)
                .OnDelete(DeleteBehavior.Restrict); // Define o comportamento de exclusão

            // Configura o campo de Quantidade
            builder.Property(dm => dm.quantidade)
                .IsRequired();

            // Configura o campo de Controle (medicamento controlado)
            builder.Property(dm => dm.recebido)
                .IsRequired();

            // Configura o campo de Recebido (indica se o medicamento foi recebido)
            builder.Property(dm => dm.recebido)
                .IsRequired();
        }
    }
}
