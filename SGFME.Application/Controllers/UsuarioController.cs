using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGFME.Application.DTOs;
using SGFME.Domain.Entidades;
using SGFME.Domain.Interfaces;
using SGFME.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGFME.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private IBaseService<Usuario> _baseService;
        private readonly SqlServerContext _context;

        public UsuarioController(IBaseService<Usuario> baseService, SqlServerContext context)
        {
            _baseService = baseService;
            _context = context;
        }

        private IActionResult Execute(Func<object> func)
        {
            try
            {
                var result = func();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> Create(UsuarioDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Verificar se os IDs de chaves estrangeiras existem
                    if (!_context.status.Any(st => st.id == request.idStatus))
                    {
                        return BadRequest($"Status com id {request.idStatus} não encontrado.");
                    }
                    if (!_context.perfilusuario.Any(pu => pu.id == request.idPerfilUsuario))
                    {
                        return BadRequest($"Perfil de Usuário com id {request.idPerfilUsuario} não encontrado.");
                    }
                    if (!_context.funcionario.Any(f => f.id == request.idFuncionario))
                    {
                        return BadRequest($"Funcionário com id {request.idFuncionario} não encontrado.");
                    }

                    var novoUsuario = new Usuario
                    {
                        id = request.id,
                        nomeUsuario = request.nomeUsuario,
                        senha = request.senha,
                        idStatus = request.idStatus,
                        idPerfilUsuario = request.idPerfilUsuario,
                        idFuncionario = request.idFuncionario
                    };

                    _context.usuario.Add(novoUsuario);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var createdUsuario = await _context.usuario
                        .Include(u => u.status)
                        .Include(u => u.perfilusuario)
                        .Include(u => u.funcionario)
                        .FirstOrDefaultAsync(e => e.id == novoUsuario.id);

                    return CreatedAtAction(nameof(Create), new { id = createdUsuario.id }, createdUsuario);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    var innerExceptionMessage = ex.InnerException?.Message ?? "Nenhuma exceção interna";
                    var fullExceptionMessage = $"Erro ao criar Usuario: {ex.Message} | Exceção interna: {innerExceptionMessage}";
                    Console.WriteLine(fullExceptionMessage);
                    return StatusCode(StatusCodes.Status500InternalServerError, fullExceptionMessage);
                }
            }
        }

        [HttpGet("dadosBasicos")]
        public async Task<ActionResult<List<object>>> GetBasicUsuarioData()
        {
            try
            {
                var usuarios = await _context.usuario
                    .Include(u => u.status)
                    .Include(u => u.perfilusuario)
                    .Include(u => u.funcionario)
                    .ThenInclude(f => f.estabelecimentosaude)
                    .Select(u => new
                    {
                        u.id,
                        u.nomeUsuario,
                        u.idStatus,
                        statusNome = u.status.nome,
                        perfilusuario = u.perfilusuario.nome,
                        funcionarioNome = u.funcionario.nomeCompleto,
                        estabelecimentoSaudeNome = u.funcionario.estabelecimentosaude.nomeFantasia
                    })
                    .ToListAsync();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os dados básicos dos usuários.");
            }
        }


        
        [HttpGet("FuncionariosPorEstabelecimento/{idEstabelecimentoSaude}")]
        public IActionResult GetFuncionariosPorEstabelecimento(long idEstabelecimentoSaude)
        {
            var funcionarios = _context.funcionario
                                       .Where(f => f.idEstabelecimentoSaude == idEstabelecimentoSaude)
                                       .Select(f => new
                                       {
                                           f.id,
                                           f.nomeCompleto
                                       })
                                       .ToList();

            if (!funcionarios.Any())
            {
                return NotFound("Nenhum funcionário encontrado para o estabelecimento de saúde especificado.");
            }

            return Ok(funcionarios);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, UsuarioDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var usuario = await _context.usuario
                        .FirstOrDefaultAsync(u => u.id == id);

                    if (usuario == null)
                    {
                        return NotFound("Usuário não encontrado.");
                    }

                    usuario.nomeUsuario = request.nomeUsuario;
                    usuario.senha = request.senha;
                    usuario.idStatus = request.idStatus;
                    usuario.idPerfilUsuario = request.idPerfilUsuario;
                    usuario.idFuncionario = request.idFuncionario;

                    _context.usuario.Update(usuario);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(usuario);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar Usuário.");
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var usuarioExistente = await _context.usuario.FirstOrDefaultAsync(u => u.id == id);

                    if (usuarioExistente == null)
                    {
                        return NotFound("Usuário não encontrado.");
                    }

                    _context.usuario.Remove(usuarioExistente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("Usuário excluído com sucesso.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir Usuário.");
                }
            }
        }

        [HttpGet("tipoStatus")]
        public IActionResult ObterTiposStatus()
        {
            try
            {
                var tiposStatus = _context.status.ToList();
                return Ok(tiposStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("tipoPerfilUsuario")]
        public IActionResult ObterTiposPerfilUsuario()
        {
            try
            {
                var tiposPerfilUsuario = _context.perfilusuario.ToList();
                return Ok(tiposPerfilUsuario);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("{id}/mudarStatus")]
        public async Task<IActionResult> MudarStatus(long id, [FromBody] int novoStatusId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var usuario = await _context.usuario.FirstOrDefaultAsync(u => u.id == id);
                    if (usuario == null)
                    {
                        return NotFound(new { Message = "Usuário não encontrado." });
                    }

                    var status = await _context.status.FirstOrDefaultAsync(s => s.id == novoStatusId);
                    if (status == null)
                    {
                        return NotFound(new { Message = "Status não encontrado." });
                    }

                    usuario.idStatus = novoStatusId;

                    _context.usuario.Update(usuario);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(usuario);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Erro ao mudar o status do usuário.", Details = ex.Message });
                }
            }
        }


        [HttpGet("{id}/dadosCompletos")]
        public async Task<ActionResult<Usuario>> GetCompleteUsuarioData(long id)
        {
            try
            {
                var usuario = await _context.usuario
                    .Include(p => p.funcionario)
                        .ThenInclude(c => c.estabelecimentosaude)
                    .Include(p => p.status)
                    .Include(p => p.perfilusuario)
                    
                    .FirstOrDefaultAsync(p => p.id == id);

                if (usuario == null)
                {
                    return NotFound("Usuario não encontrado.");
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os dados completos do usuario.");
            }
        }



        [HttpGet]
        public IActionResult SelecionarTodos()
        {
            return Execute(() => _baseService.Get<Usuario>());
        }
    }
}
