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

        public DbSet<Paciente> paciente { get; set; }
        public DbSet<Contato> contato { get; set; }
        public DbSet<TipoContato> tipocontato { get; set; }
        public DbSet<CorRaca> corraca { get; set; }
        public DbSet<Sexo> sexo { get; set; }
        public DbSet<Status> status { get; set; }
        public DbSet<EstadoCivil> estadocivil { get; set; }
        public DbSet<Profissao> profissao { get; set; }
        public DbSet<Endereco> endereco { get; set; }
        public DbSet<Naturalidade> naturalidade { get; set; }
        public DbSet<TipoEndereco> tipoendereco { get; set; }
        public DbSet<Pessoa> pessoa { get; set; }
        public DbSet<Cid> cid { get; set; }
        public DbSet<EstabelecimentoSaude> estabelecimentosaude { get; set; }
        public DbSet<Medico> medico { get; set; }
        public DbSet<Representante> representante { get; set; }
        public DbSet<Medicamento> medicamento { get; set; }
        public DbSet<Especialidade> especialidade { get; set; }
        public DbSet<VersaoCid> versaocid { get; set; }
        public DbSet<Funcionario> funcionario { get; set; }
        public DbSet<PerfilUsuario> perfilusuario { get; set; }
        public DbSet<Usuario> usuario { get; set; }
        public DbSet<PacienteRepresentante> pacienterepresentante { get; set; }
        public DbSet<Dispensacao> dispensacao { get; set; }
        public DbSet<DispensacaoMedicamento> dispensacaomedicamento { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var stringConexao = @"Server=GUSTACNOTE;DataBase=SGFMEv25;integrated security=true;TrustServerCertificate=True;";
            var stringConexao = @"Server=sql8005.site4now.net;DataBase=db_aa9649_sgfme;User Id=db_aa9649_sgfme_admin;Password=Gu_448228;TrustServerCertificate=True;";


            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(stringConexao);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>(new PacienteMapping().Configure);
            modelBuilder.Entity<Contato>(new ContatoMapping().Configure);
            modelBuilder.Entity<TipoContato>(new TipoContatoMapping().Configure);
            modelBuilder.Entity<CorRaca>(new CorRacaMapping().Configure);
            modelBuilder.Entity<Sexo>(new SexoMapping().Configure);
            modelBuilder.Entity<Status>(new StatusMapping().Configure);
            modelBuilder.Entity<EstadoCivil>(new EstadoCivilMapping().Configure);
            modelBuilder.Entity<Profissao>(new ProfissaoMapping().Configure);
            modelBuilder.Entity<Endereco>(new EnderecoMapping().Configure);
            modelBuilder.Entity<Naturalidade>(new NaturalidadeMapping().Configure);
            modelBuilder.Entity<TipoEndereco>(new TipoEnderecoMapping().Configure);
            modelBuilder.Entity<Pessoa>(new PessoaMapping().Configure);
            modelBuilder.Entity<Cid>(new CidMapping().Configure);
            modelBuilder.Entity<EstabelecimentoSaude>(new EstabelecimentoSaudeMapping().Configure);
            modelBuilder.Entity<Medico>(new MedicoMapping().Configure);
            modelBuilder.Entity<Representante>(new RepresentanteMapping().Configure);
            modelBuilder.Entity<Medicamento>(new MedicamentoMapping().Configure);
            modelBuilder.Entity<Especialidade>(new EspecialidadeMapping().Configure);
            modelBuilder.Entity<VersaoCid>(new VersaoCidMapping().Configure);
            modelBuilder.Entity<Especialidade>(new EspecialidadeMapping().Configure);
            modelBuilder.Entity<VersaoCid>(new VersaoCidMapping().Configure);
            modelBuilder.Entity<Funcionario>(new FuncionarioMapping().Configure);
            modelBuilder.Entity<PerfilUsuario>(new PerfilUsuarioMapping().Configure);
            modelBuilder.Entity<Usuario>(new UsuarioMapping().Configure);
            modelBuilder.Entity<PacienteRepresentante>(new PacienteRepresentanteMapping().Configure);
            modelBuilder.Entity<Dispensacao>(new DispensacaoMapping().Configure);
            modelBuilder.Entity<DispensacaoMedicamento>(new DispensacaoMedicamentoMapping().Configure);
        }
    }

}
