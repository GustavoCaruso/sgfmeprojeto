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
                .WithMany(s => s.medico)
                .HasForeignKey(r => r.idStatus)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.sexo)
                .WithMany(s => s.medico)
                .HasForeignKey(r => r.idSexo)
                .OnDelete(DeleteBehavior.Restrict);

           

            builder.HasOne(r => r.corraca)
                .WithMany(s => s.medico)
                .HasForeignKey(r => r.idCorRaca)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.estadocivil)
                .WithMany(s => s.medico)
                .HasForeignKey(r => r.idEstadoCivil)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
