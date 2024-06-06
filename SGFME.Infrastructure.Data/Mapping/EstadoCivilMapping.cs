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
    public class EstadoCivilMapping : IEntityTypeConfiguration<EstadoCivil>
    {
        public void Configure(EntityTypeBuilder<EstadoCivil> builder)
        {
            builder.ToTable("EstadoCivil"); //nome da table no banco
            builder.HasKey(p => p.id); //definição de chave primaria
            builder.Property(p => p.nome).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("nome");  //nome da coluna no bd

            builder.HasMany(s => s.paciente)
                .WithOne(r => r.estadocivil)
                .HasForeignKey(r => r.idEstadoCivil)
                .OnDelete(DeleteBehavior.Restrict); // Definir o comportamento de deleção

            builder.HasMany(s => s.medico)
               .WithOne(r => r.estadocivil)
               .HasForeignKey(r => r.idEstadoCivil)
               .OnDelete(DeleteBehavior.Restrict); // Definir o comportamento de deleção

            builder.HasMany(s => s.representante)
               .WithOne(r => r.estadocivil)
               .HasForeignKey(r => r.idEstadoCivil)
               .OnDelete(DeleteBehavior.Restrict); // Definir o comportamento de deleção

        }
    }
}
