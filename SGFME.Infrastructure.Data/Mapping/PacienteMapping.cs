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



            builder.Property(p => p.rgNumero).IsRequired() //campo requerido
                .HasColumnType("varchar(150)")  //tipo da coluna
                .HasColumnName("rgNumero");  //nome da coluna no bd

            builder.Property(p => p.rgDataEmissao)
                .HasColumnType("datetime")
                .HasColumnName("rgDataEmissao");

            builder.Property(p => p.rgOrgaoExpedidor).IsRequired() //campo requerido
                .HasColumnType("varchar(150)")  //tipo da coluna
                .HasColumnName("rgOrgaoExpedidor");  //nome da coluna no bd

            builder.Property(p => p.rgUfEmissao).IsRequired() //campo requerido
                .HasColumnType("varchar(150)")  //tipo da coluna
                .HasColumnName("rgUfEmissao");  //nome da coluna no bd




            builder.Property(p => p.cpfNumero).IsRequired() //campo requerido
                .HasColumnType("varchar(150)")  //tipo da coluna
                .HasColumnName("cpfNumero");  //nome da coluna no bd


            builder.Property(p => p.cnsNumero).IsRequired() //campo requerido
                .HasColumnType("varchar(150)")  //tipo da coluna
                .HasColumnName("cnsNumero");  //nome da coluna no bd




            builder.Property(p => p.nomeConjuge) //campo requerido
               .HasColumnType("varchar(150)")  //tipo da coluna
               .HasColumnName("nomeConjuge");  //nome da coluna no bd


            builder.Property(p => p.dataCadastro)
                .HasColumnType("datetime")
                .HasColumnName("dataCadastro");


        }
    }
}
