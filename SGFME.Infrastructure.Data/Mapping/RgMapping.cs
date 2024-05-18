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
    public class RgMapping : IEntityTypeConfiguration<Rg>
    {
        public void Configure(EntityTypeBuilder<Rg> builder)
        {
            builder.ToTable("Rg"); //nome da table no banco

            builder.HasKey(p => p.id); //definição de chave primaria

            builder.Property(p => p.numero).IsRequired() //campo requerido
                .HasColumnType("varchar(150)")  //tipo da coluna
                .HasColumnName("numero");  //nome da coluna no bd

            builder.Property(p => p.dataEmissao)
                .HasColumnType("datetime")
                .HasColumnName("dataEmissao");

            builder.Property(p => p.orgaoExpedidor).IsRequired() //campo requerido
                .HasColumnType("varchar(150)")  //tipo da coluna
                .HasColumnName("orgaoExpedidor");  //nome da coluna no bd

            builder.Property(p => p.ufEmissao).IsRequired() //campo requerido
                .HasColumnType("varchar(150)")  //tipo da coluna
                .HasColumnName("ufEmissao");  //nome da coluna no bd

        }
    }
}
