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
    public class CidMapping : IEntityTypeConfiguration<Cid>
    {
        public void Configure(EntityTypeBuilder<Cid> builder)
        {
            builder.ToTable("Cid"); //nome da table no banco

            builder.HasKey(p => p.id); //definição de chave primaria

            builder.Property(p => p.codigo).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("codigo");  //nome da coluna no bd

            builder.Property(p => p.descricao).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("descricao");  //nome da coluna no bd

            // Definir a relação com Status
            builder.HasOne(r => r.status)
                .WithMany(s => s.cid)
                .HasForeignKey(r => r.idStatus)
                .OnDelete(DeleteBehavior.Restrict);

            // Definir a relação com Status
            builder.HasOne(r => r.versaocid)
                .WithMany(s => s.cid)
                .HasForeignKey(r => r.idVersaoCid)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
