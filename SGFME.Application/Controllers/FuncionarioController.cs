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
    public class FuncionarioController : ControllerBase
    {
        private IBaseService<Funcionario> _baseService;
        private readonly SqlServerContext _context;

        public FuncionarioController(IBaseService<Funcionario> baseService, SqlServerContext context)
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
        public async Task<ActionResult<Funcionario>> Create(FuncionarioDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Verificar se os IDs de chaves estrangeiras existem
                    if (!_context.corraca.Any(cr => cr.id == request.idCorRaca))
                    {
                        return BadRequest($"CorRaca com id {request.idCorRaca} não encontrada.");
                    }
                    if (!_context.status.Any(st => st.id == request.idStatus))
                    {
                        return BadRequest($"Status com id {request.idStatus} não encontrado.");
                    }
                    if (!_context.sexo.Any(sx => sx.id == request.idSexo))
                    {
                        return BadRequest($"Sexo com id {request.idSexo} não encontrado.");
                    }
                    if (!_context.estadocivil.Any(ec => ec.id == request.idEstadoCivil))
                    {
                        return BadRequest($"Estado Civil com id {request.idEstadoCivil} não encontrado.");
                    }

                    var novoFuncionario = new Funcionario
                    {
                        id = request.id,
                        nomeCompleto = request.nomeCompleto,
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
                        crf = request.crf,
                        crfUf = request.crfUf,
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
                            funcionario = novoFuncionario,
                            discriminator = "Funcionario"
                        };
                        novoFuncionario.contato.Add(contato);
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
                            funcionario = novoFuncionario,
                            discriminator = "Funcionario"
                        };
                        novoFuncionario.endereco.Add(endereco);
                    }

                    _context.funcionario.Add(novoFuncionario);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var createdFuncionario = await _context.funcionario
                        .Include(f => f.contato)
                        .Include(f => f.endereco)
                        .Include(f => f.status)
                        .Include(f => f.sexo)
                        .Include(f => f.corraca)
                        .Include(f => f.estadocivil)
                        .FirstOrDefaultAsync(e => e.id == novoFuncionario.id);

                    return CreatedAtAction(nameof(Create), new { id = createdFuncionario.id }, createdFuncionario);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    var innerExceptionMessage = ex.InnerException?.Message ?? "Nenhuma exceção interna";
                    var fullExceptionMessage = $"Erro ao criar Funcionario: {ex.Message} | Exceção interna: {innerExceptionMessage}";
                    Console.WriteLine(fullExceptionMessage);
                    return StatusCode(StatusCodes.Status500InternalServerError, fullExceptionMessage);
                }
            }
        }

        [HttpGet("dadosBasicos")]
        public async Task<ActionResult<List<object>>> GetBasicFuncionarioData()
        {
            try
            {
                var funcionarios = await _context.funcionario
                    .Include(f => f.status)
                    .Select(f => new
                    {
                        f.id,
                        f.nomeCompleto,
                        f.dataNascimento,
                        f.cpfNumero,
                        f.rgNumero,
                        f.idStatus,
                        statusNome = f.status.nome,
                    })
                    .ToListAsync();

                return Ok(funcionarios);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os dados básicos dos funcionários.");
            }
        }

        // Endpoint para contatos de um funcionário
        [HttpGet("{id}/contatos")]
        public async Task<ActionResult<List<Contato>>> GetContatosByFuncionarioId(long id)
        {
            try
            {
                var contatos = await _context.contato
                    .Where(c => c.idFuncionario == id)
                    .Include(c => c.tipocontato)
                    .ToListAsync();

                if (contatos == null || !contatos.Any())
                {
                    return NotFound("Contatos não encontrados.");
                }

                return Ok(contatos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar contatos.");
            }
        }

        // Endpoint para endereços de um funcionário
        [HttpGet("{id}/enderecos")]
        public async Task<ActionResult<List<Endereco>>> GetEnderecosByFuncionarioId(long id)
        {
            try
            {
                var enderecos = await _context.endereco
                    .Where(e => e.idFuncionario == id)
                    .Include(e => e.tipoendereco)
                    .ToListAsync();

                if (enderecos == null || !enderecos.Any())
                {
                    return NotFound("Endereços não encontrados.");
                }

                return Ok(enderecos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar endereços.");
            }
        }

        // Endpoint para dados completos de um funcionário específico
        [HttpGet("{id}/dadosCompletos")]
        public async Task<ActionResult<Funcionario>> GetCompleteFuncionarioData(long id)
        {
            try
            {
                var funcionario = await _context.funcionario
                    .Include(f => f.contato)
                        .ThenInclude(c => c.tipocontato)
                    .Include(f => f.endereco)
                        .ThenInclude(e => e.tipoendereco)
                    .Include(f => f.status)
                    .Include(f => f.sexo)
                    .Include(f => f.corraca)
                    .Include(f => f.estadocivil)
                    .FirstOrDefaultAsync(f => f.id == id);

                if (funcionario == null)
                {
                    return NotFound("Funcionario não encontrado.");
                }

                return Ok(funcionario);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os dados completos do funcionário.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, FuncionarioDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var funcionario = await _context.funcionario
                        .Include(f => f.contato)
                        .Include(f => f.endereco)
                        .FirstOrDefaultAsync(f => f.id == id);

                    if (funcionario == null)
                    {
                        return NotFound("Funcionario não encontrado.");
                    }

                    funcionario.nomeCompleto = request.nomeCompleto;
                    funcionario.dataNascimento = request.dataNascimento;
                    funcionario.dataCadastro = request.dataCadastro;
                    funcionario.nomeMae = request.nomeMae;
                    funcionario.nomeConjuge = request.nomeConjuge;
                    funcionario.naturalidadeCidade = request.naturalidadeCidade;
                    funcionario.naturalidadeUf = request.naturalidadeUf;
                    funcionario.rgNumero = request.rgNumero;
                    funcionario.rgDataEmissao = request.rgDataEmissao;
                    funcionario.rgOrgaoExpedidor = request.rgOrgaoExpedidor;
                    funcionario.rgUfEmissao = request.rgUfEmissao;
                    funcionario.cnsNumero = request.cnsNumero;
                    funcionario.cpfNumero = request.cpfNumero;
                    funcionario.idStatus = request.idStatus;
                    funcionario.idCorRaca = request.idCorRaca;
                    funcionario.idSexo = request.idSexo;
                    funcionario.idEstadoCivil = request.idEstadoCivil;
                    funcionario.crf = request.crf;
                    funcionario.crfUf = request.crfUf;

                    // Atualizando os contatos
                    _context.contato.RemoveRange(funcionario.contato);
                    funcionario.contato.Clear();

                    foreach (var contatoDto in request.contato)
                    {
                        var contato = new Contato
                        {
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato,
                            funcionario = funcionario,
                            discriminator = "Funcionario"
                        };
                        funcionario.contato.Add(contato);
                    }

                    // Atualizando os endereços
                    _context.endereco.RemoveRange(funcionario.endereco);
                    funcionario.endereco.Clear();

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
                            funcionario = funcionario,
                            discriminator = "Funcionario"
                        };
                        funcionario.endereco.Add(endereco);
                    }

                    _context.funcionario.Update(funcionario);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(funcionario);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar Funcionario.");
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
                    var funcionarioExistente = await _context.funcionario
                        .Include(f => f.contato)
                        .Include(f => f.endereco)
                        .FirstOrDefaultAsync(f => f.id == id);

                    if (funcionarioExistente == null)
                    {
                        return NotFound("Funcionario não encontrado.");
                    }

                    _context.contato.RemoveRange(funcionarioExistente.contato);
                    _context.endereco.RemoveRange(funcionarioExistente.endereco);
                    _context.funcionario.Remove(funcionarioExistente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("Funcionario excluído com sucesso.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir Funcionario.");
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

        [HttpPatch("{id}/mudarStatus")]
        public async Task<IActionResult> MudarStatus(long id, [FromBody] int novoStatusId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var funcionario = await _context.funcionario.FirstOrDefaultAsync(f => f.id == id);
                    if (funcionario == null)
                    {
                        return NotFound(new { Message = "Funcionario não encontrado." });
                    }

                    var status = await _context.status.FirstOrDefaultAsync(s => s.id == novoStatusId);
                    if (status == null)
                    {
                        return NotFound(new { Message = "Status não encontrado." });
                    }

                    funcionario.idStatus = novoStatusId;

                    _context.funcionario.Update(funcionario);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(funcionario);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Erro ao mudar o status do funcionário.", Details = ex.Message });
                }
            }
        }

        [HttpGet]
        public IActionResult SelecionarTodos()
        {
            return Execute(() => _baseService.Get<FuncionarioModel>());
        }

        [HttpPut("{id}/contatos")]
        public async Task<IActionResult> UpdateContatos(long id, List<ContatoCreateDTO> novosContatos)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var funcionario = await _context.funcionario
                        .Include(f => f.contato)
                        .FirstOrDefaultAsync(f => f.id == id);

                    if (funcionario == null)
                    {
                        return NotFound("Funcionario não encontrado.");
                    }

                    // Validação dos novos contatos (opcional)
                    foreach (var contatoDto in novosContatos)
                    {
                        if (!_context.tipocontato.Any(tc => tc.id == contatoDto.idTipoContato))
                        {
                            return BadRequest($"Tipo de Contato com id {contatoDto.idTipoContato} não encontrado.");
                        }
                    }

                    _context.contato.RemoveRange(funcionario.contato);
                    funcionario.contato.Clear();

                    foreach (var contatoDto in novosContatos)
                    {
                        var contato = new Contato
                        {
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato,
                            funcionario = funcionario,
                            discriminator = "Funcionario"
                        };
                        funcionario.contato.Add(contato);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(funcionario.contato);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar contatos.");
                }
            }
        }

        [HttpPut("{id}/enderecos")]
        public async Task<IActionResult> UpdateEnderecos(long id, List<EnderecoCreateDTO> novosEnderecos)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var funcionario = await _context.funcionario
                        .Include(f => f.endereco)
                        .FirstOrDefaultAsync(f => f.id == id);

                    if (funcionario == null)
                    {
                        return NotFound("Funcionario não encontrado.");
                    }

                    foreach (var enderecoDto in novosEnderecos)
                    {
                        if (!_context.tipoendereco.Any(te => te.id == enderecoDto.idTipoEndereco))
                        {
                            return BadRequest($"Tipo de Endereço com id {enderecoDto.idTipoEndereco} não encontrado.");
                        }
                    }

                    _context.endereco.RemoveRange(funcionario.endereco);
                    funcionario.endereco.Clear();

                    foreach (var enderecoDto in novosEnderecos)
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
                            funcionario = funcionario,
                            discriminator = "Funcionario"
                        };
                        funcionario.endereco.Add(endereco);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(funcionario.endereco);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar endereços.");
                }
            }
        }
    }
}
