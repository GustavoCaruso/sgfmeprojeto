using Microsoft.EntityFrameworkCore;
using SGFME.Domain.Entidades;
using SGFME.Infrastructure.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Infrastructure.Data.Context
{
    public class SqlServerContext : DbContext
    {
        public SqlServerContext(DbContextOptions<SqlServerContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<Paciente> paciente { get; set; }//Replicar para as próximas entidades
        public DbSet<Contato> contato { get; set; }//Replicar para as próximas entidades
        public DbSet<TipoContato> tipocontato { get; set; }//Replicar para as próximas entidades



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var stringConexao = @"Server=DELLG3GUSTAVO;DataBase=SGFMEv12;integrated security=true;TrustServerCertificate=True;";
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(stringConexao);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>(new PacienteMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<Contato>(new ContatoMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<TipoContato>(new TipoContatoMapping().Configure);//Replicar para as próximas entidades

        }
    }
}
