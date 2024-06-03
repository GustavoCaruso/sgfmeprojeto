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
        public async Task<ActionResult<Medico>> Create(MedicoDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var novoMedico = new Medico
                    {
                        id = request.id,
                        nomeCompleto = request.nomeCompleto,
                        dataNascimento = request.dataNascimento,
                        crm = request.crm,
                        dataCadastro = request.dataCadastro,
                        nomeMae = request.nomeMae,
                        rgNumero = request.rgNumero,
                        rgDataEmissao = request.rgDataEmissao,
                        rgOrgaoExpedidor = request.rgOrgaoExpedidor,
                        rgUfEmissao = request.rgUfEmissao,
                        cnsNumero = request.cnsNumero,
                        cpfNumero = request.cpfNumero,
                        nomeConjuge = request.nomeConjuge,
                        naturalidadeCidade = request.naturalidadeCidade,
                        naturalidadeUf = request.naturalidadeUf,
                        idStatus = request.idStatus,
                        idSexo = request.idSexo,
                        idEstadoCivil = request.idEstadoCivil,
                        idCorRaca = request.idCorRaca,
                        contato = new List<Contato>(),
                        endereco = new List<Endereco>()
                    };

                    // Adicionando contatos
                    foreach (var contatoDto in request.contato)
                    {
                        var contato = new Contato
                        {
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato,
                            medico = novoMedico,
                            discriminator = "Medico"
                        };
                        novoMedico.contato.Add(contato);
                    }

                    // Adicionando endereços
                    foreach (var enderecoDto in request.endereco)
                    {
                        var endereco = new Endereco
                        {
                            idTipoEndereco = enderecoDto.idTipoEndereco,
                            logradouro = enderecoDto.logradouro,
                            numero = enderecoDto.numero,
                            complemento = enderecoDto.complemento,
                            bairro = enderecoDto.bairro,
                            cidade = enderecoDto.cidade,
                            uf = enderecoDto.uf,
                            cep = enderecoDto.cep,
                            pontoReferencia = enderecoDto.pontoReferencia,
                            medico = novoMedico,
                            discriminator = "Medico"
                        };
                        novoMedico.endereco.Add(endereco);
                    }

                    // Validar a entrada usando FluentValidation
                    var validator = new MedicoValidator();
                    var validationResult = await validator.ValidateAsync(novoMedico);

                    if (!validationResult.IsValid)
                    {
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.medico.Add(novoMedico);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var createdMedico = await _context.medico
                        .Include(m => m.contato)
                        .Include(m => m.endereco)
                        .Include(m => m.status)
                        .Include(m => m.sexo)
                        .Include(m => m.corraca)
                        .Include(m => m.estadocivil)
                        .FirstOrDefaultAsync(e => e.id == novoMedico.id);

                    return CreatedAtAction(nameof(Create), new { id = createdMedico.id }, createdMedico);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao criar Medico.");
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

        [HttpGet("selecionarEnderecos/{id}")]
        public async Task<ActionResult<List<Endereco>>> GetEnderecosByMedicoId(long id)
        {
            try
            {
                var medico = await _context.medico
                    .Include(m => m.endereco)
                    .FirstOrDefaultAsync(m => m.id == id);

                if (medico == null)
                {
                    return NotFound("Médico não encontrado.");
                }

                return Ok(medico.endereco);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar endereços.");
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

        [HttpGet("todosMedicosComContatosEEnderecos")]
        public async Task<ActionResult<List<Medico>>> GetAllMedicos()
        {
            try
            {
                var medicos = await _context.medico
                    .Include(m => m.contato)
                        .ThenInclude(c => c.tipocontato)
                    .Include(m => m.endereco)
                        .ThenInclude(e => e.tipoendereco)
                    .Include(m => m.status)
                    .Include(m => m.sexo)
                    .Include(m => m.corraca)
                    .Include(m => m.estadocivil)
                    .ToListAsync();

                return Ok(medicos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os médicos.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, MedicoDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var medico = await _context.medico
                        .Include(m => m.contato)
                        .Include(m => m.endereco)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (medico == null)
                    {
                        return NotFound("Médico não encontrado.");
                    }

                    medico.nomeCompleto = request.nomeCompleto;
                    medico.crm = request.crm;
                    medico.nomeMae = request.nomeMae;
                    medico.nomeConjuge = request.nomeConjuge;
                    medico.naturalidadeCidade = request.naturalidadeCidade;
                    medico.naturalidadeUf = request.naturalidadeUf;
                    medico.dataNascimento = request.dataNascimento;
                    medico.dataCadastro = request.dataCadastro;
                    medico.rgNumero = request.rgNumero;
                    medico.rgDataEmissao = request.rgDataEmissao;
                    medico.rgOrgaoExpedidor = request.rgOrgaoExpedidor;
                    medico.rgUfEmissao = request.rgUfEmissao;
                    medico.cnsNumero = request.cnsNumero;
                    medico.cpfNumero = request.cpfNumero;
                    medico.idStatus = request.idStatus; // Associação com Status
                    medico.idCorRaca = request.idCorRaca;
                    medico.idSexo = request.idSexo;
                    medico.idEstadoCivil = request.idEstadoCivil;

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

                    // Atualizando os endereços
                    _context.endereco.RemoveRange(medico.endereco);
                    medico.endereco.Clear();

                    foreach (var enderecoDto in request.endereco)
                    {
                        var endereco = new Endereco
                        {
                            idTipoEndereco = enderecoDto.idTipoEndereco,
                            logradouro = enderecoDto.logradouro,
                            numero = enderecoDto.numero,
                            complemento = enderecoDto.complemento,
                            bairro = enderecoDto.bairro,
                            cidade = enderecoDto.cidade,
                            uf = enderecoDto.uf,
                            cep = enderecoDto.cep,
                            pontoReferencia = enderecoDto.pontoReferencia,
                            medico = medico,
                            discriminator = "Medico"
                        };
                        medico.endereco.Add(endereco);
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
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar Medico.");
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
                    var medicoExistente = await _context.medico
                        .Include(m => m.contato)
                        .Include(m => m.endereco)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (medicoExistente == null)
                    {
                        return NotFound("Médico não encontrado.");
                    }

                    _context.contato.RemoveRange(medicoExistente.contato);
                    _context.endereco.RemoveRange(medicoExistente.endereco);
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
                    .ThenInclude(c => c.tipocontato) // Incluir tipocontato
                    .Include(m => m.endereco)
                    .ThenInclude(e => e.tipoendereco) // Incluir tipoendereco
                    .Include(m => m.status) // Incluir status
                    .Include(m => m.sexo) // Incluir status
                    .Include(m => m.estadocivil) // Incluir status
                    .Include(m => m.corraca) // Incluir status
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

        [HttpGet("tipoSexo")]
        public IActionResult ObterTiposSexo()
        {
            try
            {
                var tiposSexo = _context.sexo.ToList();
                return Ok(tiposSexo);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("tipoEstadoCivil")]
        public IActionResult ObterTiposEstadoCivil()
        {
            try
            {
                var tiposEstadoCivil = _context.estadocivil.ToList();
                return Ok(tiposEstadoCivil);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("tipoCorRaca")]
        public IActionResult ObterTiposCorRaca()
        {
            try
            {
                var tiposCorRaca = _context.corraca.ToList();
                return Ok(tiposCorRaca);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult SelecionarTodos()
        {
            return Execute(() => _baseService.Get<MedicoModel>());
        }
    }
}
