﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Infrastructure.Data.Mapping
{
    public class FuncionarioMapping : IEntityTypeConfiguration<Funcionario>
    {
        public void Configure(EntityTypeBuilder<Funcionario> builder)
        {
            builder.ToTable("Funcionario"); //Nome da tabela no SqlServer

            builder.HasKey(prop => prop.id); //Chave primária no SqlServer

            builder.Property(prop => prop.nomeCompleto)
                .HasConversion(prop => prop.ToString(), prop => prop)
                .IsRequired()
                .HasColumnName("nomeCompleto")
                .HasColumnType("varchar(255)");


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




            builder.Property(prop => prop.crf)
                .HasConversion(prop => prop.ToString(), prop => prop)
                .HasColumnName("crf")
                .HasColumnType("varchar(255)");

            builder.Property(prop => prop.crfUf)
                .HasConversion(prop => prop.ToString(), prop => prop)
                .HasColumnName("crfUf")
                .HasColumnType("varchar(255)");




            

            //Relações com as demais tabelas

            // Definir a relação com Status
            builder.HasOne(r => r.status)
                .WithMany(s => s.funcionario)
                .HasForeignKey(r => r.idStatus)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.sexo)
                .WithMany(s => s.funcionario)
                .HasForeignKey(r => r.idSexo)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(r => r.corraca)
                .WithMany(s => s.funcionario)
                .HasForeignKey(r => r.idCorRaca)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.estadocivil)
                .WithMany(s => s.funcionario)
                .HasForeignKey(r => r.idEstadoCivil)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.estabelecimentosaude)
                .WithMany(s => s.funcionario)
                .HasForeignKey(r => r.idEstabelecimentoSaude)
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
