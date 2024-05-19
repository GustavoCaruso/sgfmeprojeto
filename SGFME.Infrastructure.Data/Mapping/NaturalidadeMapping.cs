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
    public class NaturalidadeMapping : IEntityTypeConfiguration<Naturalidade>
    {
        public void Configure(EntityTypeBuilder<Naturalidade> builder)
        {
            builder.ToTable("Naturalidade"); //nome da table no banco

            builder.HasKey(p => p.id); //definição de chave primaria

            builder.Property(p => p.cidade).IsRequired() //campo requerido
                .HasColumnType("varchar(150)")  //tipo da coluna
                .HasColumnName("cidade");  //nome da coluna no bd

            builder.Property(p => p.uf).IsRequired() //campo requerido
                .HasColumnType("varchar(150)")  //tipo da coluna
                .HasColumnName("uf");  //nome da coluna no bd

        }
    }
}
