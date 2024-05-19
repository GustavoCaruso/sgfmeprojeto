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
    public class EnderecoMapping : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.ToTable("Endereco"); //nome da table no banco

            builder.HasKey(p => p.id); //definição de chave primaria

            builder.Property(p => p.logradouro).IsRequired() //campo requerido
                .HasColumnType("varchar(200)")  //tipo da coluna
                .HasColumnName("logradouro");  //nome da coluna no bd

            builder.Property(p => p.numero).IsRequired() //campo requerido
                .HasColumnType("varchar(20)")  //tipo da coluna
                .HasColumnName("numero");  //nome da coluna no bd

            builder.Property(p => p.complemento) 
               .HasColumnType("varchar(20)")  //tipo da coluna
               .HasColumnName("complemento");  //nome da coluna no bd

            builder.Property(p => p.bairro).IsRequired() //campo requerido
               .HasColumnType("varchar(200)")  //tipo da coluna
               .HasColumnName("bairro");  //nome da coluna no bd

            builder.Property(p => p.cidade).IsRequired() //campo requerido
               .HasColumnType("varchar(200)")  //tipo da coluna
               .HasColumnName("cidade");  //nome da coluna no bd

            builder.Property(p => p.uf).IsRequired() //campo requerido
               .HasColumnType("varchar(200)")  //tipo da coluna
               .HasColumnName("uf");  //nome da coluna no bd

            builder.Property(p => p.cep).IsRequired() //campo requerido
               .HasColumnType("varchar(200)")  //tipo da coluna
               .HasColumnName("cep");  //nome da coluna no bd

            builder.Property(p => p.pontoReferencia) //campo requerido
               .HasColumnType("varchar(200)")  //tipo da coluna
               .HasColumnName("pontoReferencia");  //nome da coluna no bd

        }
    }
}
