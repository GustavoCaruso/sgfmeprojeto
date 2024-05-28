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
    public class MedicoController : ControllerBase
    {
        private IBaseService<Medico> _baseService;
        private readonly SqlServerContext _context;

        public MedicoController(IBaseService<Medico> baseService, SqlServerContext context)
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
        public async Task<ActionResult<List<Medico>>> Create(MedicoDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Convertendo MedicoDTO para Medico
                    var novoMedico = new Medico
                    {
                        id = request.id,
                        nomeCompleto = request.nomeCompleto,
                        dataNascimento = request.dataNascimento,
                        crm = request.crm,
                        // Inicializa a lista de contatos
                        contato = new List<Contato>()
                    };
                    foreach (var contatoDto in request.contato)
                    {
                        var contato = new Contato
                        {
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato, // Define o tipo de contato
                            medico = novoMedico,
                            discriminator = "Medico", // Define o discriminador
                        };
                        novoMedico.contato.Add(contato);
                    }

                    // Validar a entrada usando FluentValidation
                    var validator = new MedicoValidator();
                    var validationResult = await validator.ValidateAsync(novoMedico); // Passando o novoMedico ao invés de request

                    if (!validationResult.IsValid)
                    {
                        // Se a entrada não for válida, retornar uma resposta de erro com as mensagens de validação
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.medico.Add(novoMedico);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var createdMedico = await _context.medico
                        .Include(p => p.contato)
                        .FirstOrDefaultAsync(e => e.id == novoMedico.id);

                    return CreatedAtAction(nameof(Create), new { id = createdMedico.id }, createdMedico);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log the exception (consider using a logging framework)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao criar Médico.");
                }
            }
        }




        [HttpGet("selecionarContatos/{id}")]
        public async Task<ActionResult<List<Contato>>> GetContatosByMedicoId(long id)
        {
            try
            {
                var medico = await _context.medico
                    .Include(m => m.contato)
                    .FirstOrDefaultAsync(m => m.id == id);

                if (medico == null)
                {
                    return NotFound("Médico não encontrado.");
                }

                return Ok(medico.contato);
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






        [HttpGet("todosMedicosComContatos")]
        public async Task<ActionResult<List<Medico>>> GetAllMedicos()
        {
            try
            {
                var medicos = await _context.medico
                    .Include(m => m.contato)
                    .ThenInclude(c => c.tipocontato)
                    .ToListAsync();

                return Ok(medicos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os médicos.");
            }
        }












        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MedicoDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var medico = await _context.medico
                        .Include(m => m.contato)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (medico == null)
                    {
                        return NotFound("Médico não encontrado.");
                    }

                    medico.nomeCompleto = request.nomeCompleto;
                    medico.dataNascimento = request.dataNascimento;
                    medico.crm = request.crm;

                    // Atualizando os contatos
                    _context.contato.RemoveRange(medico.contato);
                    medico.contato.Clear();

                    foreach (var contatoDto in request.contato)
                    {
                        var contato = new Contato
                        {
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato,
                            medico = medico,
                            discriminator = "Medico"
                        };
                        medico.contato.Add(contato);
                    }

                    // Validar a entrada usando FluentValidation
                    var validator = new MedicoValidator();
                    var validationResult = await validator.ValidateAsync(medico);

                    if (!validationResult.IsValid)
                    {
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.medico.Update(medico);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(medico);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar Médico.");
                }
            }
        }


        // Método para excluir um médico
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var medicoExistente = await _context.medico
                        .Include(m => m.contato)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (medicoExistente == null)
                    {
                        return NotFound("Médico não encontrado.");
                    }

                    _context.contato.RemoveRange(medicoExistente.contato);
                    _context.medico.Remove(medicoExistente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("Médico excluído com sucesso.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir Médico.");
                }
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicoById(int id)
        {
            try
            {
                var medico = await _context.medico
                    .Include(m => m.contato)
                    .ThenInclude(c => c.tipocontato) // Assuming tipoContato is the navigation property
                    .FirstOrDefaultAsync(m => m.id == id);

                if (medico == null)
                {
                    return NotFound("Médico não encontrado.");
                }

                return Ok(medico);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar o médico.");
            }
        }




        [HttpGet]
        public IActionResult selecionarTodos()
        {
            //select * from produtos
            return Execute(() => _baseService.Get<MedicoModel>());
        }

    }
}
