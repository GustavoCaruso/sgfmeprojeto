using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public DbSet<CorRaca> corraca { get; set; }//Replicar para as próximas entidades
        public DbSet<Sexo> sexo { get; set; }//Replicar para as próximas entidades
        public DbSet<Status> status { get; set; }//Replicar para as próximas entidades
        public DbSet<EstadoCivil> estadocivil { get; set; }//Replicar para as próximas entidades
        public DbSet<Profissao> profissao { get; set; }//Replicar para as próximas entidades
        public DbSet<Endereco> endereco { get; set; }//Replicar para as próximas entidades
        public DbSet<Naturalidade> naturalidade { get; set; }//Replicar para as próximas entidades
        public DbSet<TipoEndereco> tipoendereco { get; set; }//Replicar para as próximas entidades
        public DbSet<Pessoa> pessoa { get; set; }//Replicar para as próximas entidades
        public DbSet<Cid> cid { get; set; }//Replicar para as próximas entidades
        public DbSet<EstabelecimentoSaude> estabelecimentosaude { get; set; }//Replicar para as próximas entidades
        public DbSet<Medico> medico { get; set; }//Replicar para as próximas entidades
        public DbSet<Representante> representante { get; set; }
        public DbSet<Medicamento> medicamento { get; set; }
        public DbSet<Especialidade> especialidade { get; set; }






        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var stringConexao = @"Server=DELLG3GUSTAVO;DataBase=SGFMEv42;integrated security=true;TrustServerCertificate=True;";
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(stringConexao)
                    .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                    .EnableSensitiveDataLogging();
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>(new PacienteMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<Contato>(new ContatoMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<TipoContato>(new TipoContatoMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<CorRaca>(new CorRacaMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<Sexo>(new SexoMapping().Configure);//Replicar para as próximas entidades
           
            modelBuilder.Entity<Status>(new StatusMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<EstadoCivil>(new EstadoCivilMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<Profissao>(new ProfissaoMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<Endereco>(new EnderecoMapping().Configure);//Replicar para as próximas entidades
            
            modelBuilder.Entity<Naturalidade>(new NaturalidadeMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<TipoEndereco>(new TipoEnderecoMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<Pessoa>(new PessoaMapping().Configure);//Replicar para as próximas entidades

            modelBuilder.Entity<Cid>(new CidMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<EstabelecimentoSaude>(new EstabelecimentoSaudeMapping().Configure);//Replicar para as próximas entidades
            modelBuilder.Entity<Medico>(new MedicoMapping().Configure);//Replicar para as próximas entidades

            modelBuilder.Entity<Representante>(new RepresentanteMapping().Configure);   
            modelBuilder.Entity<Medicamento>(new MedicamentoMapping().Configure);
            modelBuilder.Entity<Especialidade>(new EspecialidadeMapping().Configure);

        }
    }
}
