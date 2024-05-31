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
    public class RepresentanteMapping : IEntityTypeConfiguration<Representante>
    {
        public void Configure(EntityTypeBuilder<Representante> builder)
        {
            builder.ToTable("Representante");

            builder.HasKey(prop => prop.id);

            builder.Property(prop => prop.nomeCompleto)
            .HasConversion(prop => prop.ToString(), prop => prop)
            .IsRequired()
            .HasColumnName("nomeCompleto")
            .HasColumnType("varchar(150)");

            builder.Property(prop => prop.dataNascimento)
            .IsRequired()
            .HasColumnName("dataNascimento")
            .HasColumnType("datetime");

            builder.Property(p => p.dataCadastro)
                .HasColumnType("datetime")
                .HasColumnName("dataCadastro");

            builder.Property(p => p.rgNumero).IsRequired() 
                .HasColumnType("varchar(150)")  
                .HasColumnName("rgNumero");  

            builder.Property(p => p.rgDataEmissao)
                .HasColumnType("datetime")
                .HasColumnName("rgDataEmissao");

            builder.Property(p => p.rgOrgaoExpedidor).IsRequired() 
                .HasColumnType("varchar(150)")  
                .HasColumnName("rgOrgaoExpedidor");  

            builder.Property(p => p.rgUfEmissao).IsRequired() 
                .HasColumnType("varchar(150)")  
                .HasColumnName("rgUfEmissao");  

            builder.Property(p => p.cpfNumero).IsRequired() 
                .HasColumnType("varchar(150)")  
                .HasColumnName("cpfNumero");  

            builder.Property(p => p.cnsNumero).IsRequired() 
                .HasColumnType("varchar(150)") 
                .HasColumnName("cnsNumero");

            // Definir a relação com Status
            builder.HasOne(r => r.status)
                .WithMany(s => s.representante)
                .HasForeignKey(r => r.idStatus)
                .OnDelete(DeleteBehavior.Restrict);






        }
    }
}