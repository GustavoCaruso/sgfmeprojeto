using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using SGFME.Application.DTOs;
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

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {


            // Configuração do CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });


            // Configuração da Autenticação JWT
            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

            services.AddControllers();


            
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SGFME.Application", Version = "v1" });
            });
            //services.AddCors(); // Make sure you call this previous to AddMvc
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddDbContext<SqlServerContext>();

            services.AddControllers().AddNewtonsoftJson(); //json
            services.AddMvc().AddNewtonsoftJson(opt =>
            {

                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            });

            services.AddSingleton(new MapperConfiguration(config =>
            {


                config.CreateMap<Paciente, PacienteModel>().ReverseMap();
                config.CreateMap<Contato, ContatoModel>().ReverseMap();
                config.CreateMap<TipoContato, TipoContatoModel>().ReverseMap();
                config.CreateMap<CorRaca, CorRacaModel>().ReverseMap();
                config.CreateMap<Sexo, SexoModel>().ReverseMap();
                config.CreateMap<Status, StatusModel>().ReverseMap();
                config.CreateMap<EstadoCivil, EstadoCivilModel>().ReverseMap();
                config.CreateMap<Profissao, ProfissaoModel>().ReverseMap();
                config.CreateMap<Endereco, EnderecoModel>().ReverseMap();
                config.CreateMap<Naturalidade, NaturalidadeModel>().ReverseMap();
                config.CreateMap<TipoEndereco, TipoEnderecoModel>().ReverseMap();
                config.CreateMap<Pessoa, PessoaModel>().ReverseMap();
                config.CreateMap<Cid, CidModel>().ReverseMap();
                config.CreateMap<EstabelecimentoSaude, EstabelecimentoSaudeModel>().ReverseMap();
                config.CreateMap<Medico, MedicoModel>().ReverseMap();
                config.CreateMap<Representante, RepresentanteModel>().ReverseMap();
                config.CreateMap<Medicamento, MedicamentoModel>().ReverseMap();
                config.CreateMap<VersaoCid, VersaoCidModel>().ReverseMap();
                config.CreateMap<Funcionario, FuncionarioModel>().ReverseMap();
                config.CreateMap<PerfilUsuario, PerfilUsuarioModel>().ReverseMap();
                config.CreateMap<Usuario, UsuarioModel>().ReverseMap();
                config.CreateMap<Dispensacao, DispensacaoModel>().ReverseMap();
                config.CreateMap<CriarDispensacaoDTO, Dispensacao>().ReverseMap();
                config.CreateMap<CriarDispensacaoMedicamentoDTO, DispensacaoMedicamento>().ReverseMap();
                config.CreateMap<AlterarDispensacaoDTO, Dispensacao>().ReverseMap();
                config.CreateMap<AlterarDispensacaoMedicamentoDTO, DispensacaoMedicamento>().ReverseMap();

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

            services.AddScoped<IBaseService<Representante>, BaseService<Representante>>();
            services.AddScoped<IBaseRepository<Representante>, BaseRepository<Representante>>();

            services.AddScoped<IBaseService<Medicamento>, BaseService<Medicamento>>();
            services.AddScoped<IBaseRepository<Medicamento>, BaseRepository<Medicamento>>();

            services.AddScoped<IBaseService<VersaoCid>, BaseService<VersaoCid>>();
            services.AddScoped<IBaseRepository<VersaoCid>, BaseRepository<VersaoCid>>();

            services.AddScoped<IBaseService<Funcionario>, BaseService<Funcionario>>();
            services.AddScoped<IBaseRepository<Funcionario>, BaseRepository<Funcionario>>();

            services.AddScoped<IBaseService<PerfilUsuario>, BaseService<PerfilUsuario>>();
            services.AddScoped<IBaseRepository<PerfilUsuario>, BaseRepository<PerfilUsuario>>();

            services.AddScoped<IBaseService<Usuario>, BaseService<Usuario>>();
            services.AddScoped<IBaseRepository<Usuario>, BaseRepository<Usuario>>();

            services.AddScoped<IBaseService<Dispensacao>, BaseService<Dispensacao>>();
            services.AddScoped<IBaseRepository<Dispensacao>, BaseRepository<Dispensacao>>();
        }

        public void Configure(WebApplication app, IWebHostEnvironment environment)
        {

            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // Ativando CORS
            app.UseCors("CorsPolicy");

            // Ativando Autenticação e Autorização
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });




            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGFME.Application v1"));
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Serve arquivos estáticos da pasta wwwroot por padrão
            

           

            

            app.MapControllers();

            
        }
    }
}
