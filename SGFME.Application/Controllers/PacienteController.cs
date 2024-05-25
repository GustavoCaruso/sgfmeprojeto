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
    public class PacienteController : ControllerBase
    {
        private readonly IBaseService<Paciente> _baseService;
        private readonly SqlServerContext _context;

        public PacienteController(IBaseService<Paciente> baseService, SqlServerContext context)
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
        public async Task<ActionResult<List<Paciente>>> Create(PacienteDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var novoPaciente = new Paciente
                    {
                        nomeCompleto = request.nomeCompleto,
                        peso = request.peso,
                        altura = request.altura,
                        dataNascimento = request.dataNascimento,
                        idade = request.idade,
                        nomeMae = request.nomeMae,
                        rgNumero = request.rgNumero,
                        rgOrgaoExpedidor = request.rgOrgaoExpedidor,
                        rgDataEmissao = request.rgDataEmissao,
                        rgUfEmissao = request.rgUfEmissao,
                        cpfNumero = request.cpfNumero,
                        cnsNumero = request.cnsNumero,
                        nomeConjuge = request.nomeConjuge,
                        dataCadastro = request.dataCadastro,
                        contato = new List<Contato>(),
                        endereco = new List<Endereco>()
                    };

                    foreach (var contatoDto in request.contato)
                    {
                        var contato = new Contato
                        {
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato,
                            paciente = novoPaciente,
                            discriminator = "Paciente", // Define o discriminador
                        };
                        novoPaciente.contato.Add(contato);
                    }

                    foreach (var enderecoDto in request.endereco)
                    {
                        var endereco = new Endereco
                        {
                            logradouro = enderecoDto.logradouro,
                            numero = enderecoDto.numero,
                            complemento = enderecoDto.complemento,
                            bairro = enderecoDto.bairro,
                            cidade = enderecoDto.cidade,
                            uf = enderecoDto.uf,
                            cep = enderecoDto.cep,
                            pontoReferencia = enderecoDto.pontoReferencia,
                            idTipoEndereco = enderecoDto.idTipoEndereco,
                            paciente = novoPaciente,
                            discriminator = "Paciente", // Define o discriminador
                        };
                        novoPaciente.endereco.Add(endereco);
                    }

                    _context.paciente.Add(novoPaciente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(await _context.paciente.Include(p => p.contato).Include(p => p.endereco).ToListAsync());
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao criar paciente.");
                }
            }
        }

        [HttpGet("{id}/contatos")]
        public async Task<IActionResult> GetContatosByPacienteId(long id)
        {
            try
            {
                var paciente = await _context.paciente
                    .Include(p => p.contato)
                    .FirstOrDefaultAsync(p => p.id == id);

                if (paciente == null)
                {
                    return NotFound("Paciente não encontrado.");
                }

                return Ok(paciente.contato);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar contatos do paciente.");
            }
        }

        [HttpGet("{id}/enderecos")]
        public async Task<IActionResult> GetEnderecosByPacienteId(long id)
        {
            try
            {
                var paciente = await _context.paciente
                    .Include(p => p.endereco)
                    .FirstOrDefaultAsync(p => p.id == id);

                if (paciente == null)
                {
                    return NotFound("Paciente não encontrado.");
                }

                return Ok(paciente.endereco);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar endereços do paciente.");
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

        [HttpGet("tipoEndereco")]
        public IActionResult ObterTiposEndereco()
        {
            try
            {
                var tiposEndereco = _context.tipoendereco.ToList();
                return Ok(tiposEndereco);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        public IActionResult SelecionarTodos()
        {
            return Execute(() => _baseService.Get<PacienteModel>());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PacienteDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var paciente = await _context.paciente
                        .Include(p => p.contato)
                        .FirstOrDefaultAsync(p => p.id == id);

                    if (paciente == null)
                    {
                        return NotFound("Paciente não encontrado.");
                    }

                    paciente.nomeCompleto = request.nomeCompleto;
                    paciente.dataNascimento = request.dataNascimento;
                    paciente.rgNumero = request.rgNumero;
                    paciente.rgOrgaoExpedidor = request.rgOrgaoExpedidor;
                    paciente.rgDataEmissao = request.rgDataEmissao;
                    paciente.rgUfEmissao = request.rgUfEmissao;
                    paciente.cpfNumero = request.cpfNumero;

                    // Atualizando os contatos
                    _context.contato.RemoveRange(paciente.contato);
                    paciente.contato.Clear();

                    foreach (var contatoDto in request.contato)
                    {
                        var contato = new Contato
                        {
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato,
                            paciente = paciente
                        };
                        paciente.contato.Add(contato);
                    }

                    // Validar a entrada usando FluentValidation
                    var validator = new PacienteValidator();
                    var validationResult = await validator.ValidateAsync(paciente);

                    if (!validationResult.IsValid)
                    {
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.paciente.Update(paciente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(paciente);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar Paciente.");
                }
            }
        }

        // Método para excluir um paciente
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var pacienteExistente = await _context.paciente.Include(p => p.contato).FirstOrDefaultAsync(p => p.id == id);

                    if (pacienteExistente == null)
                    {
                        return NotFound("Paciente não encontrado.");
                    }

                    _context.paciente.Remove(pacienteExistente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("Paciente excluído com sucesso.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir Paciente.");
                }
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPacienteById(int id)
        {
            try
            {
                var paciente = await _context.paciente
                    .Include(p => p.contato)
                    .FirstOrDefaultAsync(p => p.id == id);

                if (paciente == null)
                {
                    return NotFound("Paciente não encontrado.");
                }

                return Ok(paciente);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar o paciente.");
            }
        }

    }
}
