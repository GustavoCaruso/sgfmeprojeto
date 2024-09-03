using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGFME.Domain.Entidades;

namespace SGFME.Infrastructure.Data.Mapping
{
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario"); // Nome da tabela no SqlServer

            builder.HasKey(prop => prop.id); // Chave primária no SqlServer

            builder.Property(prop => prop.nomeUsuario)
                .IsRequired()
                .HasColumnName("nomeUsuario")
                .HasColumnType("varchar(255)");

            builder.Property(prop => prop.senha)
                .IsRequired()
                .HasColumnName("senha")
                .HasColumnType("varchar(255)");

            // Mapeamento da nova coluna PrecisaTrocarSenha
            builder.Property(prop => prop.precisaTrocarSenha)
                .IsRequired() // Campo obrigatório
                .HasDefaultValue(true) // Valor padrão true para novos usuários
                .HasColumnName("precisaTrocarSenha") // Nome da coluna no banco de dados
                .HasColumnType("bit"); // Tipo de dado booleano no SQL Server

            // Relações com as demais tabelas

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
