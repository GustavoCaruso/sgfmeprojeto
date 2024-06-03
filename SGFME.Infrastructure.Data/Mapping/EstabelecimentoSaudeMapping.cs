using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Infrastructure.Data.Mapping
{
    public class EstabelecimentoSaudeMapping : IEntityTypeConfiguration<EstabelecimentoSaude>
    {
        public void Configure(EntityTypeBuilder<EstabelecimentoSaude> builder)
        {
            builder.ToTable("EstabelecimentoSaude"); //nome da table no banco

            builder.HasKey(p => p.id); //definição de chave primaria

            builder.Property(p => p.nomeFantasia).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("nomeFantasia");  //nome da coluna no bd

            builder.Property(p => p.razaoSocial).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("razaoSocial");  //nome da coluna no bd

            builder.Property(p => p.cnes).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("cnes");  //nome da coluna no bd


            // Definir a relação com Status
            builder.HasOne(r => r.status)
                .WithMany(s => s.estabelecimentosaude)
                .HasForeignKey(r => r.idStatus)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
