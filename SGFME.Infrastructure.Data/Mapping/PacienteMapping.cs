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

            builder.Property(prop => prop.idade)
            .IsRequired()
            .HasColumnName("idade")
            .HasColumnType("int");

            builder.Property(prop => prop.nomeMae)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("nomeMae")
            .HasColumnType("varchar(150)");



            builder.HasOne(p => p.cns)
                .WithOne(c => c.paciente)
                .HasForeignKey<Paciente>(p => p.idCns)
                .OnDelete(DeleteBehavior.Cascade); // ou .Restrict baseado em sua regra de negócio


            builder.HasOne(p => p.rg)
               .WithOne(c => c.paciente)
               .HasForeignKey<Paciente>(p => p.idRg)
               .OnDelete(DeleteBehavior.Cascade); // ou .Restrict baseado em sua regra de negócio



        }
    }
}
