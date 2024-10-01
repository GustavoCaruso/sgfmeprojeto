using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Infrastructure.Data.Mapping
{
    public class TipoProcessoMapping : IEntityTypeConfiguration<TipoProcesso>
    {
        public void Configure(EntityTypeBuilder<TipoProcesso> builder)
        {
            builder.ToTable("TipoProcesso"); //nome da table no banco

            builder.HasKey(p => p.id); //definição de chave primaria

            builder.Property(p => p.nome).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("nome");  //nome da coluna no bd

           

        }
    }
}
