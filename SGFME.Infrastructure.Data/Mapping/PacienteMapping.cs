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
    public class PacienteMapping : IEntityTypeConfiguration<Paciente>
    {
        public void Configure(EntityTypeBuilder<Paciente> builder) 
        {
            builder.ToTable("Paciente"); //Nome da tabela no SqlServer

            builder.HasKey(prop => prop.id); //Chave primária no SqlServer

            builder.Property(prop => prop.nomeCompleto)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("nomeCompleto")
            .HasColumnType("varchar(150)");

            builder.Property(prop => prop.sexo)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("sexo")
            .HasColumnType("varchar(50)");

            builder.Property(prop => prop.rg)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("rg")
            .HasColumnType("varchar(8)");

            builder.Property(prop => prop.cpf)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("cpf")
            .HasColumnType("varchar(11)");

            builder.Property(prop => prop.cns)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("cns")
            .HasColumnType("varchar(15)");

            builder.Property(prop => prop.peso)
            .IsRequired()
            .HasColumnName("peso")
            .HasColumnType("decimal(8,2)");


            builder.Property(prop => prop.altura)
            .IsRequired()
            .HasColumnName("altura")
            .HasColumnType("decimal(8,2)");

            builder.Property(prop => prop.dataNascimento)
            .IsRequired()
            .HasColumnName("dataNascimento")
            .HasColumnType("datetime");

            builder.Property(prop => prop.naturalidade)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("naturalidade")
            .HasColumnType("varchar(150)");

            builder.Property(prop => prop.ufNaturalidade)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("ufNaturalidade")
            .HasColumnType("varchar(2)");

            builder.Property(prop => prop.corRaca)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("corRaca")
            .HasColumnType("varchar(50)");

            builder.Property(prop => prop.estadoCivil)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("estadoCivil")
            .HasColumnType("varchar(50)");

            builder.Property(prop => prop.nomeMae)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("nomeMae")
            .HasColumnType("varchar(150)");



        }
    }
}
