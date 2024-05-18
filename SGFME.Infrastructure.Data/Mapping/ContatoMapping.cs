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
    public class ContatoMapping : IEntityTypeConfiguration<Contato>
    {
        public void Configure(EntityTypeBuilder<Contato> builder)
        {
            builder.ToTable("Contato"); //Nome da tabela no SqlServer

            builder.HasKey(prop => prop.id); //Chave primária no SqlServer

            

            builder.Property(prop => prop.valor)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("valor")
            .HasColumnType("varchar(150)");


            //relacionamento com categoria
            builder.HasOne(p => p.tipocontato).WithMany(c => c.contato)
                .HasConstraintName("fk_tipocontato_contato")
                .HasForeignKey(p => p.idTipoContato)
                .OnDelete(DeleteBehavior.NoAction);



            //Relação com a tabela de Paciente 
            builder.HasOne(p => p.paciente).WithMany(c => c.contato)
                .HasConstraintName("fk_paciente_contato")
                .HasForeignKey(p => p.idPaciente)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
