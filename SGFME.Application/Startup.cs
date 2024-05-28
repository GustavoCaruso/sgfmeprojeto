using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using SGFME.Application.Models;
using SGFME.Domain.Entidades;
using SGFME.Domain.Interfaces;
using SGFME.Infrastructure.Data.Context;
using SGFME.Infrastructure.Data.Repository;
using SGFME.Service.Services;
using System.Text;

namespace SGFME.Application
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SGFME.Application", Version = "v1" });
            });
            //services.AddCors(); // Make sure you call this previous to AddMvc
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddDbContext<SqlServerContext>(); //contexto
            services.AddControllers().AddNewtonsoftJson(); //json
            services.AddMvc().AddNewtonsoftJson(opt =>
            {

                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            });

            services.AddSingleton(new MapperConfiguration(config =>
            {


                config.CreateMap<Paciente, PacienteModel>();
                config.CreateMap<PacienteModel, Paciente>();

                config.CreateMap<Contato, ContatoModel>();
                config.CreateMap<ContatoModel, Contato>();

                config.CreateMap<TipoContato, TipoContatoModel>();
                config.CreateMap<TipoContatoModel, TipoContato>();

                config.CreateMap<CorRaca, CorRacaModel>();
                config.CreateMap<CorRacaModel, CorRaca>();

                config.CreateMap<Sexo, SexoModel>();
                config.CreateMap<SexoModel, Sexo>();

                

                config.CreateMap<Status, StatusModel>();
                config.CreateMap<StatusModel, Status>();

                config.CreateMap<EstadoCivil, EstadoCivilModel>();
                config.CreateMap<EstadoCivilModel, EstadoCivil>();

                config.CreateMap<Profissao, ProfissaoModel>();
                config.CreateMap<ProfissaoModel, Profissao>();

                config.CreateMap<Endereco, EnderecoModel>();
                config.CreateMap<EnderecoModel, Endereco>();

               

                config.CreateMap<Naturalidade, NaturalidadeModel>();
                config.CreateMap<NaturalidadeModel, Naturalidade>();

                config.CreateMap<TipoEndereco, TipoEnderecoModel>();
                config.CreateMap<TipoEnderecoModel, TipoEndereco>();

                config.CreateMap<Pessoa, PessoaModel>();
                config.CreateMap<PessoaModel, Pessoa>();

                config.CreateMap<Cid, CidModel>();
                config.CreateMap<CidModel, Cid>();

                config.CreateMap<EstabelecimentoSaude, EstabelecimentoSaudeModel>();
                config.CreateMap<EstabelecimentoSaudeModel, EstabelecimentoSaude>();

                config.CreateMap<Medico, MedicoModel>();
                config.CreateMap<MedicoModel, Medico>();

                





            }).CreateMapper());

            services.AddScoped<IBaseService<Paciente>, BaseService<Paciente>>();
            services.AddScoped<IBaseRepository<Paciente>, BaseRepository<Paciente>>();

            services.AddScoped<IBaseService<Contato>, BaseService<Contato>>();
            services.AddScoped<IBaseRepository<Contato>, BaseRepository<Contato>>();

            services.AddScoped<IBaseService<TipoContato>, BaseService<TipoContato>>();
            services.AddScoped<IBaseRepository<TipoContato>, BaseRepository<TipoContato>>();

            services.AddScoped<IBaseService<CorRaca>, BaseService<CorRaca>>();
            services.AddScoped<IBaseRepository<CorRaca>, BaseRepository<CorRaca>>();

            services.AddScoped<IBaseService<Sexo>, BaseService<Sexo>>();
            services.AddScoped<IBaseRepository<Sexo>, BaseRepository<Sexo>>();

           

            services.AddScoped<IBaseService<Status>, BaseService<Status>>();
            services.AddScoped<IBaseRepository<Status>, BaseRepository<Status>>();

            services.AddScoped<IBaseService<EstadoCivil>, BaseService<EstadoCivil>>();
            services.AddScoped<IBaseRepository<EstadoCivil>, BaseRepository<EstadoCivil>>();

            services.AddScoped<IBaseService<Profissao>, BaseService<Profissao>>();
            services.AddScoped<IBaseRepository<Profissao>, BaseRepository<Profissao>>();

            services.AddScoped<IBaseService<Endereco>, BaseService<Endereco>>();
            services.AddScoped<IBaseRepository<Endereco>, BaseRepository<Endereco>>();

           

            services.AddScoped<IBaseService<Naturalidade>, BaseService<Naturalidade>>();
            services.AddScoped<IBaseRepository<Naturalidade>, BaseRepository<Naturalidade>>();

            services.AddScoped<IBaseService<TipoEndereco>, BaseService<TipoEndereco>>();
            services.AddScoped<IBaseRepository<TipoEndereco>, BaseRepository<TipoEndereco>>();

            services.AddScoped<IBaseService<Pessoa>, BaseService<Pessoa>>();
            services.AddScoped<IBaseRepository<Pessoa>, BaseRepository<Pessoa>>();

            services.AddScoped<IBaseService<Cid>, BaseService<Cid>>();
            services.AddScoped<IBaseRepository<Cid>, BaseRepository<Cid>>();

            services.AddScoped<IBaseService<EstabelecimentoSaude>, BaseService<EstabelecimentoSaude>>();
            services.AddScoped<IBaseRepository<EstabelecimentoSaude>, BaseRepository<EstabelecimentoSaude>>();

            services.AddScoped<IBaseService<Medico>, BaseService<Medico>>();
            services.AddScoped<IBaseRepository<Medico>, BaseRepository<Medico>>();

            

        }

        public void Configure(WebApplication app, IWebHostEnvironment environment)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGFME.Application v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
