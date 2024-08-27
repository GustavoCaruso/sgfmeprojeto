using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SGFME.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Infrastructure.Data.Mapping
{
    public class PerfilUsuarioMapping
    {
        public void Configure(EntityTypeBuilder<PerfilUsuario> builder)
        {
            builder.ToTable("PerfilUsuario"); //nome da table no banco
            builder.HasKey(p => p.id); //definição de chave primaria
            builder.Property(p => p.nome).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("nome");  //nome da coluna no bd


            // Definir a relação com Representante
            builder.HasMany(s => s.usuario)
                .WithOne(r => r.perfilusuario)
                .HasForeignKey(r => r.idPerfilUsuario)
                .OnDelete(DeleteBehavior.Restrict); // Definir o comportamento de deleção
        }
    }
}
