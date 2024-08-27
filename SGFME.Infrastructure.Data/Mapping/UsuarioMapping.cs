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
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario"); //Nome da tabela no SqlServer

            builder.HasKey(prop => prop.id); //Chave primária no SqlServer

            builder.Property(prop => prop.nomeUsuario)
                .HasConversion(prop => prop.ToString(), prop => prop)
                .IsRequired()
                .HasColumnName("nomeUsuario")
                .HasColumnType("varchar(255)");

            builder.Property(prop => prop.senha)
                .HasConversion(prop => prop.ToString(), prop => prop)
                .IsRequired()
                .HasColumnName("senha")
                .HasColumnType("varchar(255)");


            //Relações com as demais tabelas

            // Definir a relação com Status
            builder.HasOne(r => r.status)
                .WithMany(s => s.usuario)
                .HasForeignKey(r => r.idStatus)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.perfilusuario)
                .WithMany(s => s.usuario)
                .HasForeignKey(r => r.idPerfilUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.funcionario)
                .WithMany(s => s.usuario)
                .HasForeignKey(r => r.idFuncionario)
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
