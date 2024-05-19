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
    public class CpfMapping : IEntityTypeConfiguration<Cpf>
    {
        public void Configure(EntityTypeBuilder<Cpf> builder)
        {
            builder.ToTable("Cpf"); //nome da table no banco

            builder.HasKey(p => p.id); //definição de chave primaria

            builder.Property(p => p.numero).IsRequired() //campo requerido
                .HasColumnType("varchar(150)")  //tipo da coluna
                .HasColumnName("numero");  //nome da coluna no bd

            

            

        }
    }
}
