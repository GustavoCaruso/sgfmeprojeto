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
    public class MedicamentoMapping : IEntityTypeConfiguration<Medicamento>
    {

        public void Configure(EntityTypeBuilder<Medicamento> builder)
        {


            builder.ToTable("Medicamento"); //nome da table no banco

            builder.HasKey(p => p.id); //definição de chave primaria

            builder.Property(p => p.nome).IsRequired() //campo requerido
                .HasColumnType("varchar(255)")  //tipo da coluna
                .HasColumnName("nomeFantasia");  //nome da coluna no bd
        }
    }
}

