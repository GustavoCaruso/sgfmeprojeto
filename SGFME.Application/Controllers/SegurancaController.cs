using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SGFME.Application.DTOs;
using SGFME.Application.Models;
using SGFME.Domain.Entidades;
using SGFME.Domain.Interfaces;
using SGFME.Infrastructure.Data.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SGFME.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SegurancaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IBaseService<Usuario> _service;
        private readonly ILogger<Usuario> _logger;
        private readonly SqlServerContext _context;

        public SegurancaController(IConfiguration configuration, IBaseService<Usuario> service, ILogger<Usuario> logger, SqlServerContext context)
        {
            _configuration = configuration;
            _service = service;
            _logger = logger;
            _context = context;
        }




        [HttpPost]
        [Route("validaLogin")]
        public IActionResult Login([FromBody] dynamic loginDetalhes)
        {
            string nomeUsuario = loginDetalhes.nomeUsuario;
            string senha = loginDetalhes.senha;

            if (string.IsNullOrEmpty(nomeUsuario) || string.IsNullOrEmpty(senha))
            {
                return BadRequest("Nome de usuário ou senha não podem estar vazios.");
            }

            var usuario = ValidarUsuario(nomeUsuario, senha);

            if (usuario != null)
            {
                if (usuario.status == null || usuario.status.nome.Trim().ToLower() != "ativo")
                {
                    return Unauthorized(new
                    {
                        mensagem = "Usuário não está ativo.",
                        statusUsuario = usuario.status?.nome
                    });
                }

                // Mapeamento de UsuarioDTO para UsuarioModel
                var usuarioModel = new UsuarioModel
                {
                    id = usuario.id,
                    nomeUsuario = usuario.nomeUsuario,
                    senha = usuario.senha,
                    idStatus = usuario.idStatus,
                    idPerfilUsuario = usuario.idPerfilUsuario,
                    idFuncionario = usuario.idFuncionario
                };

                var tokenString = GerarTokenJWT(usuarioModel);
                return Ok(new
                {
                    token = tokenString,
                    id = usuarioModel.id,
                    nome = usuarioModel.nomeUsuario,
                    statusUsuario = usuario.status?.nome // Inclui o status na resposta para depuração
                });
            }
            else
            {
                return Unauthorized("Usuário ou senha inválidos.");
            }
        }

        private Usuario ValidarUsuario(string nomeUsuario, string senha)
        {
            return _context.usuario
                .Include(u => u.status)
                .FirstOrDefault(u => u.nomeUsuario == nomeUsuario && u.senha == senha);
        }










        private string GerarTokenJWT(UsuarioModel usuario)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.nomeUsuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                // Adicione outros claims conforme necessário
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
