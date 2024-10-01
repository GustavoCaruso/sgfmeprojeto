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
            builder.ToTable("DispensacaoMedicamento");

            // Configura a chave primária
            builder.HasKey(dm => dm.id);

            // Configura o relacionamento com Medicamento (muitos para um)
            builder.HasOne(dm => dm.medicamento)
                .WithMany()
                .HasForeignKey(dm => dm.idMedicamento)
                .OnDelete(DeleteBehavior.Restrict); // Define o comportamento de exclusão

            // Configura o campo de Quantidade
            builder.Property(dm => dm.quantidade)
                .IsRequired();

            // Configura o campo de Recibo para medicamentos de tarja preta
            builder.Property(dm => dm.recibo)
                .IsRequired();

            // Configura o campo de Receita para medicamentos normais
            builder.Property(dm => dm.receita)
                .IsRequired();

            // Configura o campo de Medicamento Chegou
            builder.Property(dm => dm.medicamentoChegou)
                .IsRequired();

            // Configura o campo de Medicamento Entregue
            builder.Property(dm => dm.medicamentoEntregue)
                .IsRequired();

            // Configura o campo de Data de Entrega
            builder.Property(dm => dm.dataEntrega)
                .IsRequired(false); // Campo opcional

          
        }
    }
}
