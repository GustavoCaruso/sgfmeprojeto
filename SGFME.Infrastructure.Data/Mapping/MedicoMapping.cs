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
    public class MedicoMapping : IEntityTypeConfiguration<Medico>
    {
        public void Configure(EntityTypeBuilder<Medico> builder)
        {
            builder.ToTable("Medico"); //nome da table no banco

            builder.HasKey(p => p.id); //definição de chave primaria
            builder.Property(p => p.nomeCompleto).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("nomeCompleto");  //nome da coluna no bd

            builder.Property(p => p.dataNascimento)
                           .IsRequired() // Campo requerido
                           .HasColumnType("datetime")  // Tipo da coluna (remover parêntese extra)
                           .HasColumnName("dataNascimento");  // Nome da coluna no banco

            builder.Property(p => p.crm).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("crm");  //nome da coluna no bd

        }
    }
}
