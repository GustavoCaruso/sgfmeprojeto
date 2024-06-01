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
    public class CorRacaMapping : IEntityTypeConfiguration<CorRaca>
    {
        public void Configure(EntityTypeBuilder<CorRaca> builder)
        {
            builder.ToTable("CorRaca"); //nome da table no banco
            builder.HasKey(p => p.id); //definição de chave primaria
            builder.Property(p => p.nome).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("nome");  //nome da coluna no bd

            builder.HasMany(s => s.paciente)
                .WithOne(r => r.corraca)
                .HasForeignKey(r => r.idCorRaca)
                .OnDelete(DeleteBehavior.Restrict); // Definir o comportamento de deleção

        }
    }
}
