﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjetoConsultorio.Application", Version = "v1" });
            });
            services.AddCors(); // Make sure you call this previous to AddMvc
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

                

            }).CreateMapper());

            services.AddScoped<IBaseService<Paciente>, BaseService<Paciente>>();
            services.AddScoped<IBaseRepository<Paciente>, BaseRepository<Paciente>>();

        }

        public void Configure(WebApplication app, IWebHostEnvironment environment)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjetoConsultorio.Application v1"));
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
