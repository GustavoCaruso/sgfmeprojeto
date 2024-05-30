using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGFME.Application.DTOs;
using SGFME.Application.Models;
using SGFME.Domain.Entidades;
using SGFME.Domain.Interfaces;
using SGFME.Infrastructure.Data.Context;
using SGFME.Service.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGFME.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepresentanteController : ControllerBase
    {
        private IBaseService<Representante> _baseService;
        private readonly SqlServerContext _context;

        public RepresentanteController(IBaseService<Representante> baseService, SqlServerContext context)
        {
            _baseService = baseService;
            _context = context;
        }

        // Adicionar método para executar comando e retornar IActionResult
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
        public async Task<ActionResult<List<Representante>>> Create(RepresentanteDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Convertendo RepresentanteDTO para Representante
                    var novoRepresentante = new Representante
                    {
                        id = request.id,
                        nomeCompleto = request.nomeCompleto,
                        dataNascimento = request.dataNascimento,
                        dataCadastro = request.dataCadastro,
                        rgNumero = request.rgNumero,
                        rgDataEmissao = request.rgDataEmissao,
                        rgOrgaoExpedidor = request.rgOrgaoExpedidor,
                        rgUfEmissao = request.rgUfEmissao,
                        cnsNumero = request.cnsNumero,
                        cpfNumero = request.cpfNumero,
                        // Inicializa a lista de contatos
                        contato = new List<Contato>()
                    };

                    foreach (var contatoDto in request.contato)
                    {
                        var contato = new Contato
                        {
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato,
                            representante = novoRepresentante,
                            discriminator = "Representante"
                        };
                        novoRepresentante.contato.Add(contato);
                    }

                    // Validar a entrada usando FluentValidation
                    var validator = new RepresentanteValidator();
                    var validationResult = await validator.ValidateAsync(novoRepresentante);

                    if (!validationResult.IsValid)
                    {
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.representante.Add(novoRepresentante);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var createdRepresentante = await _context.representante
                        .Include(p => p.contato)
                        .FirstOrDefaultAsync(e => e.id == novoRepresentante.id);

                    return CreatedAtAction(nameof(Create), new { id = createdRepresentante.id }, createdRepresentante);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao criar Representante.");
                }
            }
        }

        [HttpGet("selecionarContatos/{id}")]
        public async Task<ActionResult<List<Contato>>> GetContatosByRepresentanteId(long id)
        {
            try
            {
                var representante = await _context.representante
                    .Include(m => m.contato)
                    .FirstOrDefaultAsync(m => m.id == id);

                if (representante == null)
                {
                    return NotFound("Representante não encontrado.");
                }

                return Ok(representante.contato);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar contatos.");
            }
        }

        [HttpGet("tipoContato")]
        public IActionResult ObterTiposContato()
        {
            try
            {
                var tiposContato = _context.tipocontato.ToList();
                return Ok(tiposContato);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("todosRepresentantesComContatos")]
        public async Task<ActionResult<List<Representante>>> GetAllRepresentantes()
        {
            try
            {
                var representantes = await _context.representante
                    .Include(m => m.contato)
                    .ThenInclude(c => c.tipocontato)
                    .ToListAsync();

                return Ok(representantes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os representantes.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, RepresentanteDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var representante = await _context.representante
                        .Include(m => m.contato)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (representante == null)
                    {
                        return NotFound("Representante não encontrado.");
                    }

                    representante.nomeCompleto = request.nomeCompleto;
                    representante.dataNascimento = request.dataNascimento;
                    representante.dataCadastro = request.dataCadastro;
                    representante.rgNumero = request.rgNumero;
                    representante.rgDataEmissao = request.rgDataEmissao;
                    representante.rgOrgaoExpedidor = request.rgOrgaoExpedidor;
                    representante.rgUfEmissao = request.rgUfEmissao;
                    representante.cnsNumero = request.cnsNumero;
                    representante.cpfNumero = request.cpfNumero;

                    // Atualizando os contatos
                    _context.contato.RemoveRange(representante.contato);
                    representante.contato.Clear();

                    foreach (var contatoDto in request.contato)
                    {
                        var contato = new Contato
                        {
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato,
                            representante = representante,
                            discriminator = "Representante"
                        };
                        representante.contato.Add(contato);
                    }

                    // Validar a entrada usando FluentValidation
                    var validator = new RepresentanteValidator();
                    var validationResult = await validator.ValidateAsync(representante);

                    if (!validationResult.IsValid)
                    {
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.representante.Update(representante);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(representante);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar Representante.");
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
                    var representanteExistente = await _context.representante
                        .Include(m => m.contato)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (representanteExistente == null)
                    {
                        return NotFound("Representante não encontrado.");
                    }

                    _context.contato.RemoveRange(representanteExistente.contato);
                    _context.representante.Remove(representanteExistente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("Representante excluído com sucesso.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir Representante.");
                }
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRepresentanteById(int id)
        {
            try
            {
                var representante = await _context.representante
                    .Include(m => m.contato)
                    .ThenInclude(c => c.tipocontato)
                    .FirstOrDefaultAsync(m => m.id == id);

                if (representante == null)
                {
                    return NotFound("Representante não encontrado.");
                }

                return Ok(representante);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar o representante.");
            }
        }

        [HttpGet]
        public IActionResult SelecionarTodos()
        {
            return Execute(() => _baseService.Get<RepresentanteModel>());
        }
    }
}
