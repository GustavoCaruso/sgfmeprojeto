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
                .HasColumnType("varchar(255)");

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

            builder.Property(p => p.dataCadastro)
                .HasColumnType("datetime")
                .HasColumnName("dataCadastro");

            builder.Property(prop => prop.nomeMae)
                .HasConversion(prop => prop.ToString(), prop => prop)
                .IsRequired()
                .HasColumnName("nomeMae")
                .HasColumnType("varchar(255)");



            builder.Property(p => p.rgNumero).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("rgNumero");  //nome da coluna no bd

            builder.Property(p => p.rgDataEmissao)
                .HasColumnType("datetime")
                .HasColumnName("rgDataEmissao");

            builder.Property(p => p.rgOrgaoExpedidor).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("rgOrgaoExpedidor");  //nome da coluna no bd

            builder.Property(p => p.rgUfEmissao).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("rgUfEmissao");  //nome da coluna no bd




            builder.Property(p => p.cpfNumero).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("cpfNumero");  //nome da coluna no bd


            builder.Property(p => p.cnsNumero).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("cnsNumero");  //nome da coluna no bd




            builder.Property(p => p.nomeConjuge) //campo requerido
               .HasColumnType("varchar(255)")  //tipo da coluna
               .HasColumnName("nomeConjuge");  //nome da coluna no bd


            builder.Property(prop => prop.naturalidadeCidade)
                .HasConversion(prop => prop.ToString(), prop => prop)
                .IsRequired()
                .HasColumnName("naturalidadeCidade")
                .HasColumnType("varchar(255)");

            builder.Property(prop => prop.naturalidadeUf)
                .HasConversion(prop => prop.ToString(), prop => prop)
                .IsRequired()
                .HasColumnName("naturalidadeUf")
                .HasColumnType("varchar(255)");




            //Relações com as demais tabelas

            // Definir a relação com Status
            builder.HasOne(r => r.status)
                .WithMany(s => s.paciente)
                .HasForeignKey(r => r.idStatus)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.sexo)
                .WithMany(s => s.paciente)
                .HasForeignKey(r => r.idSexo)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.profissao)
                .WithMany(s => s.paciente)
                .HasForeignKey(r => r.idProfissao)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.corraca)
                .WithMany(s => s.paciente)
                .HasForeignKey(r => r.idCorRaca)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.estadocivil)
                .WithMany(s => s.paciente)
                .HasForeignKey(r => r.idEstadoCivil)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
