using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGFME.Application.DTOs;
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
        private readonly IBaseService<EstabelecimentoSaude> _baseService;
        private readonly SqlServerContext _context;

        public EstabelecimentoSaudeController(IBaseService<EstabelecimentoSaude> baseService, SqlServerContext context)
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
                        dataCadastro = request.dataCadastro,
                        idStatus = request.idStatus,
                        contato = new List<Contato>(),
                        endereco = new List<Endereco>()
                    };

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




        [HttpGet("dadosBasicos")]
        public async Task<ActionResult<List<object>>> GetBasicEstabelecimentoSaudeData()
        {
            try
            {
                var estabelecimentos = await _context.estabelecimentosaude
                    .Include(e => e.status) // Inclui o relacionamento com Status
                    .Select(e => new
                    {
                        e.id,
                        e.nomeFantasia,
                        e.razaoSocial,
                        e.cnes,
                        e.dataCadastro,
                        e.idStatus, // Inclui o idStatus
                        statusNome = e.status.nome // Inclui o nome do status
                    })
                    .ToListAsync(); // Remove paginação

                return Ok(estabelecimentos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os dados básicos dos estabelecimentos de saúde.");
            }
        }


        // Endpoint para contatos de um estabelecimento de saúde
        [HttpGet("{id}/contatos")]
        public async Task<ActionResult<List<Contato>>> GetContatosByEstabelecimentoSaudeId(long id)
        {
            try
            {
                var contatos = await _context.contato
                    .Where(c => c.idEstabelecimentoSaude == id) // Filtra pelos contatos associados ao estabelecimento de saúde
                    .Include(c => c.tipocontato) // Inclui o relacionamento com TipoContato
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



        // Endpoint para endereços de um estabelecimento de saúde
        [HttpGet("{id}/enderecos")]
        public async Task<ActionResult<List<Endereco>>> GetEnderecosByEstabelecimentoSaudeId(long id)
        {
            try
            {
                var enderecos = await _context.endereco
                    .Where(e => e.idEstabelecimentoSaude == id) // Filtra pelos endereços associados ao estabelecimento de saúde
                    .Include(e => e.tipoendereco) // Inclui o relacionamento com TipoEndereco
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



        // Endpoint para dados completos de um estabelecimento de saúde específico
        [HttpGet("{id}/dadosCompletos")]
        public async Task<ActionResult<EstabelecimentoSaude>> GetCompleteEstabelecimentoSaudeData(long id)
        {
            try
            {
                var estabelecimentoSaude = await _context.estabelecimentosaude
                    .Include(es => es.contato)
                        .ThenInclude(c => c.tipocontato)
                    .Include(es => es.endereco)
                        .ThenInclude(e => e.tipoendereco)
                    .Include(es => es.status)
                    .FirstOrDefaultAsync(es => es.id == id);

                if (estabelecimentoSaude == null)
                {
                    return NotFound("Estabelecimento de saúde não encontrado.");
                }

                return Ok(estabelecimentoSaude);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os dados completos do estabelecimento de saúde.");
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
                    estabelecimento.dataCadastro = request.dataCadastro;
                    estabelecimento.idStatus = request.idStatus;

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
            return Execute(() => _baseService.Get<EstabelecimentoSaude>());
        }

        [HttpPatch("{id}/mudarStatus")]
        public async Task<IActionResult> MudarStatusEstabelecimentoSaude(long id, [FromBody] int novoStatusId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var estabelecimentoSaude = await _context.estabelecimentosaude.FirstOrDefaultAsync(es => es.id == id);
                    if (estabelecimentoSaude == null)
                    {
                        return NotFound(new { Message = "Estabelecimento de saúde não encontrado." });
                    }

                    var status = await _context.status.FirstOrDefaultAsync(s => s.id == novoStatusId);
                    if (status == null)
                    {
                        return NotFound(new { Message = "Status não encontrado." });
                    }

                    // Validação de regras de negócio, se aplicável
                    // if (!estabelecimentoSaude.CanChangeStatusTo(novoStatusId))
                    // {
                    //     return BadRequest(new { Message = "Mudança de status inválida." });
                    // }

                    estabelecimentoSaude.idStatus = novoStatusId;

                    // Validação utilizando FluentValidation (se aplicável)
                    // var validator = new EstabelecimentoSaudeValidator();
                    // var validationResult = await validator.ValidateAsync(estabelecimentoSaude);
                    // if (!validationResult.IsValid)
                    // {
                    //     return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    // }

                    _context.estabelecimentosaude.Update(estabelecimentoSaude);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(estabelecimentoSaude);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log ex para diagnóstico
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Erro ao mudar o status do estabelecimento de saúde.", Details = ex.Message });
                }
            }
        }


        [HttpPut("{id}/contatos")]
        public async Task<IActionResult> UpdateContatosEstabelecimentoSaude(long id, List<ContatoCreateDTO> novosContatos)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var estabelecimentoSaude = await _context.estabelecimentosaude
                        .Include(es => es.contato)
                        .FirstOrDefaultAsync(es => es.id == id);

                    if (estabelecimentoSaude == null)
                    {
                        return NotFound("Estabelecimento de saúde não encontrado.");
                    }

                    // Validação dos novos contatos (opcional)
                    foreach (var contatoDto in novosContatos)
                    {
                        if (!_context.tipocontato.Any(tc => tc.id == contatoDto.idTipoContato))
                        {
                            return BadRequest($"Tipo de Contato com id {contatoDto.idTipoContato} não encontrado.");
                        }
                    }

                    // Remove todos os contatos antigos
                    _context.contato.RemoveRange(estabelecimentoSaude.contato);
                    estabelecimentoSaude.contato.Clear();

                    // Adiciona os novos contatos
                    foreach (var contatoDto in novosContatos)
                    {
                        var contato = new Contato
                        {
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato,
                            estabelecimentosaude = estabelecimentoSaude,
                            discriminator = "EstabelecimentoSaude"
                        };
                        estabelecimentoSaude.contato.Add(contato);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(estabelecimentoSaude.contato);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar contatos.");
                }
            }
        }


        [HttpPut("{id}/enderecos")]
        public async Task<IActionResult> UpdateEnderecosEstabelecimentoSaude(long id, List<EnderecoCreateDTO> novosEnderecos)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var estabelecimentoSaude = await _context.estabelecimentosaude
                        .Include(es => es.endereco)
                        .FirstOrDefaultAsync(es => es.id == id);

                    if (estabelecimentoSaude == null)
                    {
                        return NotFound("Estabelecimento de saúde não encontrado.");
                    }

                    // Validação dos novos endereços (opcional)
                    foreach (var enderecoDto in novosEnderecos)
                    {
                        if (!_context.tipoendereco.Any(te => te.id == enderecoDto.idTipoEndereco))
                        {
                            return BadRequest($"Tipo de Endereço com id {enderecoDto.idTipoEndereco} não encontrado.");
                        }
                    }

                    // Remove todos os endereços antigos
                    _context.endereco.RemoveRange(estabelecimentoSaude.endereco);
                    estabelecimentoSaude.endereco.Clear();

                    // Adiciona os novos endereços
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
                            estabelecimentosaude = estabelecimentoSaude,
                            discriminator = "EstabelecimentoSaude"
                        };
                        estabelecimentoSaude.endereco.Add(endereco);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(estabelecimentoSaude.endereco);
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
