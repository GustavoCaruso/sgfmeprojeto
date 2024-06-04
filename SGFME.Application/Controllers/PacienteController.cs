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
        private IBaseService<Paciente> _baseService;
        private readonly SqlServerContext _context;

        public PacienteController(IBaseService<Paciente> baseService, SqlServerContext context)
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
        public async Task<ActionResult<Paciente>> Create(PacienteDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    
                    var novoPaciente = new Paciente
                    {
                        id = request.id,
                        nomeCompleto = request.nomeCompleto,
                        peso = request.peso,
                        altura = request.altura,
                        dataNascimento = request.dataNascimento,
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
                        idProfissao = request.idProfissao,
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
                            paciente = novoPaciente,
                            discriminator = "Paciente"
                        };
                        novoPaciente.contato.Add(contato);
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
                            paciente = novoPaciente,
                            discriminator = "Paciente"
                        };
                        novoPaciente.endereco.Add(endereco);
                    }

                    // Validar a entrada usando FluentValidation
                    var validator = new PacienteValidator();
                    var validationResult = await validator.ValidateAsync(novoPaciente);

                    if (!validationResult.IsValid)
                    {
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.paciente.Add(novoPaciente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var createdPaciente = await _context.paciente
                        .Include(p => p.contato)
                        .Include(p => p.endereco)
                        .Include(p => p.status)
                        .Include(p => p.sexo)
                        .Include(p => p.corraca)
                        .Include(p => p.profissao)
                        .Include(p => p.estadocivil)
                        .FirstOrDefaultAsync(e => e.id == novoPaciente.id);

                    return CreatedAtAction(nameof(Create), new { id = createdPaciente.id }, createdPaciente);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao criar Paciente.");
                }
            }
        }


        [HttpGet("selecionarContatos/{id}")]
        public async Task<ActionResult<List<Contato>>> GetContatosByPacienteId(long id)
        {
            try
            {
                var paciente = await _context.paciente
                    .Include(m => m.contato)
                    .FirstOrDefaultAsync(m => m.id == id);

                if (paciente == null)
                {
                    return NotFound("paciente não encontrado.");
                }

                return Ok(paciente.contato);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar contatos.");
            }
        }

        [HttpGet("selecionarEnderecos/{id}")]
        public async Task<ActionResult<List<Endereco>>> GetEnderecosByPacienteId(long id)
        {
            try
            {
                var paciente = await _context.paciente
                    .Include(m => m.endereco)
                    .FirstOrDefaultAsync(m => m.id == id);

                if (paciente == null)
                {
                    return NotFound("Paciente não encontrado.");
                }

                return Ok(paciente.endereco);
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

        [HttpGet("todosPacientesComContatosEEnderecos")]
        public async Task<ActionResult<List<Paciente>>> GetAllPacientes()
        {
            try
            {
                var pacientes = await _context.paciente
                    .Include(m => m.contato)
                        .ThenInclude(c => c.tipocontato)
                    .Include(m => m.endereco)
                        .ThenInclude(e => e.tipoendereco)
                    .Include(m => m.status)
                    .Include(m => m.sexo)
                    .Include(m => m.corraca)
                    .Include(m => m.profissao)
                    .Include(m => m.estadocivil)
                    .ToListAsync();

                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os pacientes.");
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, PacienteDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var paciente = await _context.paciente
                        .Include(m => m.contato)
                        .Include(m => m.endereco)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (paciente == null)
                    {
                        return NotFound("paciente não encontrado.");
                    }

                    paciente.nomeCompleto = request.nomeCompleto;
                    paciente.peso = request.peso;
                    paciente.altura = request.altura;
                    paciente.nomeMae = request.nomeMae;
                    paciente.nomeConjuge = request.nomeConjuge;
                    paciente.naturalidadeCidade = request.naturalidadeCidade;
                    paciente.naturalidadeUf = request.naturalidadeUf;
                    paciente.dataNascimento = request.dataNascimento;
                    paciente.dataCadastro = request.dataCadastro;
                    paciente.rgNumero = request.rgNumero;
                    paciente.rgDataEmissao = request.rgDataEmissao;
                    paciente.rgOrgaoExpedidor = request.rgOrgaoExpedidor;
                    paciente.rgUfEmissao = request.rgUfEmissao;
                    paciente.cnsNumero = request.cnsNumero;
                    paciente.cpfNumero = request.cpfNumero;
                    paciente.idStatus = request.idStatus; // Associação com Status
                    paciente.idCorRaca = request.idCorRaca;
                    paciente.idProfissao = request.idProfissao;
                    paciente.idSexo = request.idSexo;
                    paciente.idEstadoCivil = request.idEstadoCivil;

                    // Atualizando os contatos
                    _context.contato.RemoveRange(paciente.contato);
                    paciente.contato.Clear();

                    foreach (var contatoDto in request.contato)
                    {
                        var contato = new Contato
                        {
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato,
                            paciente = paciente,
                            discriminator = "Paciente"
                        };
                        paciente.contato.Add(contato);
                    }

                    // Atualizando os endereços
                    _context.endereco.RemoveRange(paciente.endereco);
                    paciente.endereco.Clear();

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
                            paciente = paciente,
                            discriminator = "Paciente"
                        };
                        paciente.endereco.Add(endereco);
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var pacienteExistente = await _context.paciente
                        .Include(m => m.contato)
                        .Include(m => m.endereco)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (pacienteExistente == null)
                    {
                        return NotFound("Paciente não encontrado.");
                    }

                    _context.contato.RemoveRange(pacienteExistente.contato);
                    _context.endereco.RemoveRange(pacienteExistente.endereco);
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
                    .Include(m => m.contato)
                    .ThenInclude(c => c.tipocontato) // Incluir tipocontato
                    .Include(m => m.endereco)
                    .ThenInclude(e => e.tipoendereco) // Incluir tipoendereco
                    .Include(m => m.status) // Incluir status
                    .Include(m => m.sexo) // Incluir status
                    .Include(m => m.estadocivil) // Incluir status
                    .Include(m => m.profissao) // Incluir status
                    .Include(m => m.corraca) // Incluir status
                    .FirstOrDefaultAsync(m => m.id == id);

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

        [HttpGet("tipoProfissao")]
        public IActionResult ObterTiposProfissao()
        {
            try
            {
                var tiposProfissao = _context.profissao.ToList();
                return Ok(tiposProfissao);
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

    }
}
