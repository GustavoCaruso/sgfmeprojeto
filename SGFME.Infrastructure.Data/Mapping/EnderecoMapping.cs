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

            builder.Property(prop => prop.discriminator)
               .IsRequired()
               .HasMaxLength(50);


            //relacionamento com 
            builder.HasOne(p => p.tipoendereco).WithMany(c => c.endereco)
                .HasConstraintName("fk_tipoendereco_endereco")
                .HasForeignKey(p => p.idTipoEndereco)
                .OnDelete(DeleteBehavior.NoAction);
       


            //Relação com a tabela de Paciente 
            builder.HasOne(p => p.paciente).WithMany(c => c.endereco)
                .HasConstraintName("fk_paciente_endereco")
                .HasForeignKey(p => p.idPaciente)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);


            
            builder.HasOne(p => p.representante).WithMany(c => c.endereco)
                .HasConstraintName("fk_representante_endereco")
                .HasForeignKey(p => p.idRepresentante)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            builder.HasOne(p => p.medico).WithMany(c => c.endereco)
                .HasConstraintName("fk_medico_endereco")
                .HasForeignKey(p => p.idMedico)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            builder.HasOne(p => p.estabelecimentosaude).WithMany(c => c.endereco)
               .HasConstraintName("fk_estabelecimentosaude_endereco")
               .HasForeignKey(p => p.idEstabelecimentoSaude)
               .OnDelete(DeleteBehavior.NoAction)
               .IsRequired(false);

        }
    }
}
