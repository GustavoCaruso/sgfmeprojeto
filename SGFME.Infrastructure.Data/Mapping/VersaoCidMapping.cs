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
    public class VersaoCidMapping : IEntityTypeConfiguration<VersaoCid>
    {
        public void Configure(EntityTypeBuilder<VersaoCid> builder)
        {
            builder.ToTable("VersaoCid"); //nome da table no banco

            builder.HasKey(p => p.id); //definição de chave primaria

            builder.Property(p => p.nome).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("nome");  //nome da coluna no bd

            // Definir a relação com Representante
            builder.HasMany(s => s.cid)
                .WithOne(r => r.versaocid)
                .HasForeignKey(r => r.idVersaoCid)
                .OnDelete(DeleteBehavior.Restrict); // Definir o comportamento de deleção

        }
    }
}
