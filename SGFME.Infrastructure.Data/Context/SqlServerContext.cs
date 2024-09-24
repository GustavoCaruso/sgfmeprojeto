using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGFME.Domain.Entidades;
using SGFME.Infrastructure.Data.Mapping;

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
            if (!optionsBuilder.IsConfigured)
            {
                var stringConexao = @"Server=DESKTOP-6H5FN98;DataBase=SGFME;integrated security=true;TrustServerCertificate=True;";
                optionsBuilder.UseSqlServer(stringConexao);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqlServerContext).Assembly);
        }
    }
}