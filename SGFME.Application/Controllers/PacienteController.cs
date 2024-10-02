using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                    // Verificar se há representantes duplicados na lista de RGs fornecida
                    var rgsUnicos = request.rgRepresentante.Distinct().ToList();
                    if (rgsUnicos.Count != request.rgRepresentante.Count)
                    {
                        return BadRequest("Não é permitido adicionar representantes duplicados.");
                    }

                    // Buscar Representante pelo RG (um ou mais Rgs dos representantes podem ser passados no request)
                    var representantesBuscados = await _context.representante
                        .Where(r => rgsUnicos.Contains(r.rgNumero))
                        .ToListAsync();

                    // Verificar se todos os RGs fornecidos existem
                    if (representantesBuscados.Count != rgsUnicos.Count)
                    {
                        var rgsNaoEncontrados = rgsUnicos
                            .Where(rg => !representantesBuscados.Any(representante => representante.rgNumero == rg))
                            .ToList();
                        return BadRequest($"Os seguintes RGs de representantes não foram encontrados: {string.Join(", ", rgsNaoEncontrados)}");
                    }

                    if (representantesBuscados.Count > 3)
                    {
                        return BadRequest("O paciente não pode ter mais do que 3 representantes.");
                    }

                    // Criar o novo paciente
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

                    // Primeiro, salva o paciente no banco de dados
                    _context.paciente.Add(novoPaciente);
                    await _context.SaveChangesAsync(); // Isso garante que o id do paciente seja gerado

                    // Criar a relação entre Paciente e Representantes
                    foreach (var representante in representantesBuscados)
                    {
                        var pacienteRepresentante = new PacienteRepresentante
                        {
                            idPaciente = novoPaciente.id, // Usar o id gerado do paciente
                            idRepresentante = representante.id
                        };
                        _context.pacienterepresentante.Add(pacienteRepresentante);
                    }

                    // Salvar os relacionamentos no banco de dados
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
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao criar Paciente: {innerException}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Erro inesperado ao criar Paciente: {ex.Message}");
                }
            }
        }






        [HttpGet("dadosBasicos")]
        public async Task<ActionResult<List<object>>> GetBasicPacienteData()
        {
            try
            {
                var pacientes = await _context.paciente
                    .Include(p => p.status) // Inclui o relacionamento com Status
                    .Include(p => p.pacienterepresentante) // Inclui o relacionamento PacienteRepresentante
                        .ThenInclude(pr => pr.representante) // Inclui os representantes
                    .Select(p => new
                    {
                        p.id,
                        p.nomeCompleto,
                        p.dataNascimento,
                        p.cpfNumero,
                        p.rgNumero,
                        p.idStatus,
                        statusNome = p.status.nome, // Nome do status

                        // Dados dos representantes
                        representantes = p.pacienterepresentante.Select(pr => new
                        {
                            pr.representante.nomeCompleto,
                            pr.representante.rgNumero,
                            pr.representante.cpfNumero,
                            pr.representante.dataNascimento
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao buscar os dados básicos dos pacientes: {ex.Message}");
            }
        }





        // Endpoint para contatos de um paciente
        [HttpGet("{id}/contatos")]
        public async Task<ActionResult<List<Contato>>> GetContatosByPacienteId(long id)
        {
            try
            {
                var contatos = await _context.contato
                    .Where(c => c.idPaciente == id)
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

        // Endpoint para endereços de um paciente
        [HttpGet("{id}/enderecos")]
        public async Task<ActionResult<List<Endereco>>> GetEnderecosByPacienteId(long id)
        {
            try
            {
                var enderecos = await _context.endereco
                    .Where(e => e.idPaciente == id)
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


        // Endpoint para dados completos de um paciente específico
        [HttpGet("{id}/dadosCompletos")]
        public async Task<ActionResult<object>> GetCompletePacienteData(long id)
        {
            try
            {
                var paciente = await _context.paciente
                    .Include(p => p.contato)
                        .ThenInclude(c => c.tipocontato)
                    .Include(p => p.endereco)
                        .ThenInclude(e => e.tipoendereco)
                    .Include(p => p.status)
                    .Include(p => p.sexo)
                    .Include(p => p.corraca)
                    .Include(p => p.profissao)
                    .Include(p => p.estadocivil)
                    .Include(p => p.pacienterepresentante) // Inclui a relação com PacienteRepresentante
                        .ThenInclude(pr => pr.representante) // Inclui os representantes
                    .Select(p => new
                    {
                        p.id,
                        p.nomeCompleto,
                        p.peso,
                        p.altura,
                        p.dataNascimento,
                        p.dataCadastro,
                        p.nomeMae,
                        p.nomeConjuge,
                        p.rgNumero,
                        p.rgDataEmissao, // Incluindo data de emissão do RG
                        p.rgOrgaoExpedidor, // Incluindo órgão expedidor do RG
                        p.rgUfEmissao, // Incluindo UF de emissão do RG
                        p.cnsNumero, // Incluindo CNS
                        p.cpfNumero,
                        p.idStatus,
                        statusNome = p.status.nome,
                        p.idSexo,
                        sexoNome = p.sexo.nome,
                        p.idProfissao,
                        profissaoNome = p.profissao.nome,
                        p.idCorRaca,
                        corRacaNome = p.corraca.nome,
                        p.idEstadoCivil,
                        estadoCivilNome = p.estadocivil.nome,

                        // Incluindo naturalidade (UF e Cidade)
                        p.naturalidadeUf, // UF de naturalidade
                        p.naturalidadeCidade, // Cidade de naturalidade

                        // Contatos do paciente
                        contatos = p.contato.Select(c => new
                        {
                            c.valor,
                            tipoContato = c.tipocontato.nome
                        }).ToList(),

                        // Endereços do paciente
                        enderecos = p.endereco.Select(e => new
                        {
                            e.logradouro,
                            e.numero,
                            e.complemento,
                            e.bairro,
                            e.cidade,
                            e.uf,
                            e.cep,
                            e.pontoReferencia, // Incluindo ponto de referência
                            tipoEndereco = e.tipoendereco.nome
                        }).ToList(),

                        // Representantes do paciente
                        representantes = p.pacienterepresentante.Select(pr => new
                        {
                            pr.representante.nomeCompleto,
                            pr.representante.rgNumero,
                            pr.representante.cpfNumero,
                            pr.representante.dataNascimento
                        }).ToList()
                    })
                    .FirstOrDefaultAsync(p => p.id == id);

                if (paciente == null)
                {
                    return NotFound("Paciente não encontrado.");
                }

                return Ok(paciente);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao buscar os dados completos do paciente: {ex.Message}");
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
                    // Buscar o paciente existente
                    var paciente = await _context.paciente
                        .Include(p => p.contato)
                        .Include(p => p.endereco)
                        .Include(p => p.pacienterepresentante)
                        .ThenInclude(pr => pr.representante)
                        .FirstOrDefaultAsync(p => p.id == id);

                    if (paciente == null)
                    {
                        return NotFound("Paciente não encontrado.");
                    }

                    // Atualizar os dados do paciente
                    paciente.nomeCompleto = request.nomeCompleto;
                    paciente.peso = request.peso;
                    paciente.altura = request.altura;
                    paciente.dataNascimento = request.dataNascimento;
                    paciente.dataCadastro = request.dataCadastro;
                    paciente.nomeMae = request.nomeMae;
                    paciente.rgNumero = request.rgNumero;
                    paciente.rgDataEmissao = request.rgDataEmissao;
                    paciente.rgOrgaoExpedidor = request.rgOrgaoExpedidor;
                    paciente.rgUfEmissao = request.rgUfEmissao;
                    paciente.cnsNumero = request.cnsNumero;
                    paciente.cpfNumero = request.cpfNumero;
                    paciente.nomeConjuge = request.nomeConjuge;
                    paciente.naturalidadeCidade = request.naturalidadeCidade;
                    paciente.naturalidadeUf = request.naturalidadeUf;
                    paciente.idStatus = request.idStatus;
                    paciente.idSexo = request.idSexo;
                    paciente.idEstadoCivil = request.idEstadoCivil;
                    paciente.idProfissao = request.idProfissao;
                    paciente.idCorRaca = request.idCorRaca;

                    // Verificar e atualizar contatos
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

                    // Verificar e atualizar endereços
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

                    // Verificar representantes duplicados
                    var rgsUnicos = request.rgRepresentante.Distinct().ToList();
                    if (rgsUnicos.Count != request.rgRepresentante.Count)
                    {
                        return BadRequest("Não é permitido adicionar representantes duplicados.");
                    }

                    // Buscar Representantes pelos RGs
                    var representantesBuscados = await _context.representante
                        .Where(r => rgsUnicos.Contains(r.rgNumero))
                        .ToListAsync();

                    // Verificar se todos os RGs fornecidos existem
                    if (representantesBuscados.Count != rgsUnicos.Count)
                    {
                        var rgsNaoEncontrados = rgsUnicos
                            .Where(rg => !representantesBuscados.Any(representante => representante.rgNumero == rg))
                            .ToList();
                        return BadRequest($"Os seguintes RGs de representantes não foram encontrados: {string.Join(", ", rgsNaoEncontrados)}");
                    }

                    // Limpar representantes antigos e adicionar novos
                    _context.pacienterepresentante.RemoveRange(paciente.pacienterepresentante);
                    paciente.pacienterepresentante.Clear();

                    foreach (var representante in representantesBuscados)
                    {
                        var pacienteRepresentante = new PacienteRepresentante
                        {
                            idPaciente = paciente.id,
                            idRepresentante = representante.id
                        };
                        _context.pacienterepresentante.Add(pacienteRepresentante);
                    }

                    // Atualizar o paciente no banco de dados
                    _context.paciente.Update(paciente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(paciente);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao atualizar Paciente: {ex.Message}");
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


        [HttpPatch("{id}/mudarStatus")]
        public async Task<IActionResult> MudarStatus(long id, [FromBody] int novoStatusId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var paciente = await _context.paciente.FirstOrDefaultAsync(p => p.id == id);
                    if (paciente == null)
                    {
                        return NotFound(new { Message = "Paciente não encontrado." });
                    }

                    var status = await _context.status.FirstOrDefaultAsync(s => s.id == novoStatusId);
                    if (status == null)
                    {
                        return NotFound(new { Message = "Status não encontrado." });
                    }

                    // Validação de regras de negócio, se aplicável
                    // if (!paciente.CanChangeStatusTo(novoStatusId))
                    // {
                    //     return BadRequest(new { Message = "Mudança de status inválida." });
                    // }

                    paciente.idStatus = novoStatusId;

                    // Validação utilizando FluentValidation (se aplicável)
                    // var validator = new PacienteValidator();
                    // var validationResult = await validator.ValidateAsync(paciente);
                    // if (!validationResult.IsValid)
                    // {
                    //     return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    // }

                    _context.paciente.Update(paciente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(paciente);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log ex para diagnóstico
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Erro ao mudar o status do paciente.", Details = ex.Message });
                }
            }
        }



        [HttpGet]
        public IActionResult SelecionarTodos()
        {
            return Execute(() => _baseService.Get<PacienteModel>());
        }


        [HttpPut("{id}/contatos")]
        public async Task<IActionResult> UpdateContatos(long id, List<ContatoCreateDTO> novosContatos)
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

                    // Validação dos novos contatos (opcional)
                    foreach (var contatoDto in novosContatos)
                    {
                        if (!_context.tipocontato.Any(tc => tc.id == contatoDto.idTipoContato))
                        {
                            return BadRequest($"Tipo de Contato com id {contatoDto.idTipoContato} não encontrado.");
                        }
                    }

                    // Remove todos os contatos antigos
                    _context.contato.RemoveRange(paciente.contato);
                    paciente.contato.Clear();

                    // Adiciona os novos contatos
                    foreach (var contatoDto in novosContatos)
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

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(paciente.contato);
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
                    var paciente = await _context.paciente
                        .Include(p => p.endereco)
                        .FirstOrDefaultAsync(p => p.id == id);

                    if (paciente == null)
                    {
                        return NotFound("Paciente não encontrado.");
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
                    _context.endereco.RemoveRange(paciente.endereco);
                    paciente.endereco.Clear();

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
                            paciente = paciente,
                            discriminator = "Paciente"
                        };
                        paciente.endereco.Add(endereco);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(paciente.endereco);
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