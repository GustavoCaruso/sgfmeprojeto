﻿using FluentValidation;
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
        public async Task<ActionResult<Representante>> Create(RepresentanteDTO request)
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
                            representante = novoRepresentante,
                            discriminator = "Representante"
                        };
                        novoRepresentante.contato.Add(contato);
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
                            representante = novoRepresentante,
                            discriminator = "Representante"
                        };
                        novoRepresentante.endereco.Add(endereco);
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
                        .Include(p => p.endereco)
                        .Include(p => p.status)
                        .Include(p => p.sexo)
                        .Include(p => p.corraca)
                        .Include(p => p.profissao)
                        .Include(p => p.estadocivil)
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







        [HttpGet("dadosBasicos")]
        public async Task<ActionResult<List<object>>> GetBasicRepresentanteData()
        {
            try
            {
                var representantes = await _context.representante
                    .Include(r => r.status) // Inclui o relacionamento com Status
                    .Select(r => new
                    {
                        r.id,
                        r.nomeCompleto,
                        r.dataNascimento,
                        r.cpfNumero,
                        r.rgNumero,
                        r.idStatus, // Inclui o idStatus
                        statusNome = r.status.nome, // Inclui o nome do status
                    })
                    .ToListAsync(); // Remove paginação

                return Ok(representantes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os dados básicos dos representantes.");
            }
        }



        [HttpGet("{id}/contatos")]
        public async Task<ActionResult<List<Contato>>> GetContatosByRepresentanteId(long id)
        {
            try
            {
                var contatos = await _context.contato
                    .Where(c => c.idRepresentante == id) // Filtra os contatos pelo id do representante
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





        [HttpGet("{id}/enderecos")]
        public async Task<ActionResult<List<Endereco>>> GetEnderecosByRepresentanteId(long id)
        {
            try
            {
                var enderecos = await _context.endereco
                    .Where(e => e.idRepresentante == id) // Filtra os endereços pelo id do representante
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







        [HttpGet("{id}/dadosCompletos")]
        public async Task<ActionResult<Representante>> GetCompleteRepresentanteData(long id)
        {
            try
            {
                var representante = await _context.representante
                    .Include(r => r.contato)
                        .ThenInclude(c => c.tipocontato)
                    .Include(r => r.endereco)
                        .ThenInclude(e => e.tipoendereco)
                    .Include(r => r.status)
                    .Include(r => r.sexo)
                    .Include(r => r.corraca)
                    .Include(r => r.profissao)
                    .Include(r => r.estadocivil)
                    .FirstOrDefaultAsync(r => r.id == id);

                if (representante == null)
                {
                    return NotFound("Representante não encontrado.");
                }

                return Ok(representante);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os dados completos do representante.");
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

        [HttpGet("todosRepresentantesComContatosEEnderecos")]
        public async Task<ActionResult<List<Representante>>> GetAllRepresentantes()
        {
            try
            {
                var representantes = await _context.representante
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

                return Ok(representantes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os representantes.");
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, RepresentanteDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var representante = await _context.representante
                        .Include(m => m.contato)
                        .Include(m => m.endereco)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (representante == null)
                    {
                        return NotFound("Representante não encontrado.");
                    }

                    representante.nomeCompleto = request.nomeCompleto;
                    representante.nomeMae = request.nomeMae;
                    representante.nomeConjuge = request.nomeConjuge;
                    representante.naturalidadeCidade = request.naturalidadeCidade;
                    representante.naturalidadeUf = request.naturalidadeUf;
                    representante.dataNascimento = request.dataNascimento;
                    representante.dataCadastro = request.dataCadastro;
                    representante.rgNumero = request.rgNumero;
                    representante.rgDataEmissao = request.rgDataEmissao;
                    representante.rgOrgaoExpedidor = request.rgOrgaoExpedidor;
                    representante.rgUfEmissao = request.rgUfEmissao;
                    representante.cnsNumero = request.cnsNumero;
                    representante.cpfNumero = request.cpfNumero;
                    representante.idStatus = request.idStatus; // Associação com Status
                    representante.idCorRaca = request.idCorRaca;
                    representante.idProfissao = request.idProfissao;
                    representante.idSexo = request.idSexo;
                    representante.idEstadoCivil = request.idEstadoCivil;

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

                    // Atualizando os endereços
                    _context.endereco.RemoveRange(representante.endereco);
                    representante.endereco.Clear();

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
                            representante = representante,
                            discriminator = "Representante"
                        };
                        representante.endereco.Add(endereco);
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
                        .Include(m => m.endereco)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (representanteExistente == null)
                    {
                        return NotFound("Representante não encontrado.");
                    }

                    _context.contato.RemoveRange(representanteExistente.contato);
                    _context.endereco.RemoveRange(representanteExistente.endereco);
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
                    .ThenInclude(c => c.tipocontato) // Incluir tipocontato
                    .Include(m => m.endereco)
                    .ThenInclude(e => e.tipoendereco) // Incluir tipoendereco
                    .Include(m => m.status) // Incluir status
                    .Include(m => m.sexo) // Incluir status
                    .Include(m => m.estadocivil) // Incluir status
                    .Include(m => m.profissao) // Incluir status
                    .Include(m => m.corraca) // Incluir status
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
            return Execute(() => _baseService.Get<RepresentanteModel>());
        }





        [HttpPatch("{id}/mudarStatus")]
        public async Task<IActionResult> MudarStatus(long id, [FromBody] int novoStatusId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var representante = await _context.representante.FirstOrDefaultAsync(r => r.id == id);
                    if (representante == null)
                    {
                        return NotFound(new { Message = "Representante não encontrado." });
                    }

                    var status = await _context.status.FirstOrDefaultAsync(s => s.id == novoStatusId);
                    if (status == null)
                    {
                        return NotFound(new { Message = "Status não encontrado." });
                    }

                    representante.idStatus = novoStatusId;

                    _context.representante.Update(representante);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(representante);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Erro ao mudar o status do representante.", Details = ex.Message });
                }
            }
        }

        [HttpPut("{id}/contatos")]
        public async Task<IActionResult> UpdateContatos(long id, List<ContatoCreateDTO> novosContatos)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var representante = await _context.representante
                        .Include(r => r.contato)
                        .FirstOrDefaultAsync(r => r.id == id);

                    if (representante == null)
                    {
                        return NotFound("Representante não encontrado.");
                    }

                    foreach (var contatoDto in novosContatos)
                    {
                        if (!_context.tipocontato.Any(tc => tc.id == contatoDto.idTipoContato))
                        {
                            return BadRequest($"Tipo de Contato com id {contatoDto.idTipoContato} não encontrado.");
                        }
                    }

                    _context.contato.RemoveRange(representante.contato);
                    representante.contato.Clear();

                    foreach (var contatoDto in novosContatos)
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

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(representante.contato);
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
                    var representante = await _context.representante
                        .Include(r => r.endereco)
                        .FirstOrDefaultAsync(r => r.id == id);

                    if (representante == null)
                    {
                        return NotFound("Representante não encontrado.");
                    }

                    foreach (var enderecoDto in novosEnderecos)
                    {
                        if (!_context.tipoendereco.Any(te => te.id == enderecoDto.idTipoEndereco))
                        {
                            return BadRequest($"Tipo de Endereço com id {enderecoDto.idTipoEndereco} não encontrado.");
                        }
                    }

                    _context.endereco.RemoveRange(representante.endereco);
                    representante.endereco.Clear();

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
                            representante = representante,
                            discriminator = "Representante"
                        };
                        representante.endereco.Add(endereco);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(representante.endereco);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar endereços.");
                }
            }
        }

        [HttpGet("buscarPorRg/{rgNumero}")]
        public async Task<ActionResult<object>> BuscarRepresentantePorRg(string rgNumero)
        {
            try
            {
                var representante = await _context.representante
                    .Where(r => r.rgNumero == rgNumero)
                    .Select(r => new
                    {
                        r.id,
                        r.nomeCompleto,
                        r.dataNascimento,
                        r.cpfNumero,
                        r.rgNumero
                    })
                    .FirstOrDefaultAsync();

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





    }
}