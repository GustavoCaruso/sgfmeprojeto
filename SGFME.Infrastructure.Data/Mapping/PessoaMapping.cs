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
    public class PessoaMapping : IEntityTypeConfiguration<Pessoa>
    {
        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            builder.ToTable("Pessoa"); //Nome da tabela no SqlServer

            builder.HasKey(prop => prop.id); //Chave primária no SqlServer

            builder.Property(prop => prop.nomeCompleto)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("nomeCompleto")
            .HasColumnType("varchar(150)");

            builder.Property(prop => prop.dataNascimento)
            .IsRequired()
            .HasColumnName("dataNascimento")
            .HasColumnType("datetime");

            builder.Property(prop => prop.idade)
            .IsRequired()
            .HasColumnName("idade")
            .HasColumnType("int");

            builder.Property(prop => prop.nomeMae)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("nomeMae")
            .HasColumnType("varchar(150)");




        }
    }
}
