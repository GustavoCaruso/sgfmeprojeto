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
    public class EstabelecimentoSaudeController : ControllerBase
    {
        private IBaseService<EstabelecimentoSaude> _baseService;
        private readonly SqlServerContext _context;

        public EstabelecimentoSaudeController(IBaseService<EstabelecimentoSaude> baseService, SqlServerContext context)
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
        public async Task<ActionResult<EstabelecimentoSaude>> Create(EstabelecimentoSaudeDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var novoEstabelecimento = new EstabelecimentoSaude
                    {
                        id = request.id,
                        nomeFantasia = request.nomeFantasia,
                        razaoSocial = request.razaoSocial,
                        cnes = request.cnes,
                        idStatus = request.idStatus,
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
                            estabelecimentosaude = novoEstabelecimento,
                            discriminator = "EstabelecimentoSaude"
                        };
                        novoEstabelecimento.contato.Add(contato);
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
                            estabelecimentosaude = novoEstabelecimento,
                            discriminator = "EstabelecimentoSaude"
                        };
                        novoEstabelecimento.endereco.Add(endereco);
                    }

                    // Validar a entrada usando FluentValidation
                    var validator = new EstabelecimentoSaudeValidator();
                    var validationResult = await validator.ValidateAsync(novoEstabelecimento);

                    if (!validationResult.IsValid)
                    {
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.estabelecimentosaude.Add(novoEstabelecimento);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var createdEstabelecimento = await _context.estabelecimentosaude
                        .Include(e => e.contato)
                        .Include(e => e.endereco)
                        .Include(e => e.status)
                        .FirstOrDefaultAsync(e => e.id == novoEstabelecimento.id);

                    return CreatedAtAction(nameof(Create), new { id = createdEstabelecimento.id }, createdEstabelecimento);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao criar Estabelecimento de Saúde.");
                }
            }
        }

        [HttpGet("selecionarContatos/{id}")]
        public async Task<ActionResult<List<Contato>>> GetContatosByEstabelecimentoId(long id)
        {
            try
            {
                var estabelecimento = await _context.estabelecimentosaude
                    .Include(e => e.contato)
                    .FirstOrDefaultAsync(e => e.id == id);

                if (estabelecimento == null)
                {
                    return NotFound("Estabelecimento de Saúde não encontrado.");
                }

                return Ok(estabelecimento.contato);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar contatos.");
            }
        }

        [HttpGet("selecionarEnderecos/{id}")]
        public async Task<ActionResult<List<Endereco>>> GetEnderecosByEstabelecimentoId(long id)
        {
            try
            {
                var estabelecimento = await _context.estabelecimentosaude
                    .Include(e => e.endereco)
                    .FirstOrDefaultAsync(e => e.id == id);

                if (estabelecimento == null)
                {
                    return NotFound("Estabelecimento de Saúde não encontrado.");
                }

                return Ok(estabelecimento.endereco);
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

        [HttpGet("todosEstabelecimentosComContatosEEnderecos")]
        public async Task<ActionResult<List<EstabelecimentoSaude>>> GetAllEstabelecimentos()
        {
            try
            {
                var estabelecimentos = await _context.estabelecimentosaude
                    .Include(e => e.contato)
                        .ThenInclude(c => c.tipocontato)
                    .Include(e => e.endereco)
                        .ThenInclude(e => e.tipoendereco)
                    .Include(e => e.status)
                    .ToListAsync();

                return Ok(estabelecimentos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os estabelecimentos.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, EstabelecimentoSaudeDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var estabelecimento = await _context.estabelecimentosaude
                        .Include(e => e.contato)
                        .Include(e => e.endereco)
                        .FirstOrDefaultAsync(e => e.id == id);

                    if (estabelecimento == null)
                    {
                        return NotFound("Estabelecimento de Saúde não encontrado.");
                    }

                    estabelecimento.nomeFantasia = request.nomeFantasia;
                    estabelecimento.razaoSocial = request.razaoSocial;
                    estabelecimento.cnes = request.cnes;
                    estabelecimento.idStatus = request.idStatus;

                    // Atualizando os contatos
                    _context.contato.RemoveRange(estabelecimento.contato);
                    estabelecimento.contato.Clear();

                    foreach (var contatoDto in request.contato)
                    {
                        var contato = new Contato
                        {
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato,
                            estabelecimentosaude = estabelecimento,
                            discriminator = "EstabelecimentoSaude"
                        };
                        estabelecimento.contato.Add(contato);
                    }

                    // Atualizando os endereços
                    _context.endereco.RemoveRange(estabelecimento.endereco);
                    estabelecimento.endereco.Clear();

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
                            estabelecimentosaude = estabelecimento,
                            discriminator = "EstabelecimentoSaude"
                        };
                        estabelecimento.endereco.Add(endereco);
                    }

                    // Validar a entrada usando FluentValidation
                    var validator = new EstabelecimentoSaudeValidator();
                    var validationResult = await validator.ValidateAsync(estabelecimento);

                    if (!validationResult.IsValid)
                    {
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.estabelecimentosaude.Update(estabelecimento);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(estabelecimento);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar Estabelecimento de Saúde.");
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
                    var estabelecimentoExistente = await _context.estabelecimentosaude
                        .Include(e => e.contato)
                        .Include(e => e.endereco)
                        .FirstOrDefaultAsync(e => e.id == id);

                    if (estabelecimentoExistente == null)
                    {
                        return NotFound("Estabelecimento de Saúde não encontrado.");
                    }

                    _context.contato.RemoveRange(estabelecimentoExistente.contato);
                    _context.endereco.RemoveRange(estabelecimentoExistente.endereco);
                    _context.estabelecimentosaude.Remove(estabelecimentoExistente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("Estabelecimento de Saúde excluído com sucesso.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir Estabelecimento de Saúde.");
                }
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEstabelecimentoById(int id)
        {
            try
            {
                var estabelecimento = await _context.estabelecimentosaude
                    .Include(e => e.contato)
                    .ThenInclude(c => c.tipocontato)
                    .Include(e => e.endereco)
                    .ThenInclude(e => e.tipoendereco)
                    .Include(e => e.status)
                    .FirstOrDefaultAsync(e => e.id == id);

                if (estabelecimento == null)
                {
                    return NotFound("Estabelecimento de Saúde não encontrado.");
                }

                return Ok(estabelecimento);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar o estabelecimento de saúde.");
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

        [HttpGet]
        public IActionResult SelecionarTodos()
        {
            return Execute(() => _baseService.Get<EstabelecimentoSaudeModel>());
        }
    }
}
