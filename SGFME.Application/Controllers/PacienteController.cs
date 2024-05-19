using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGFME.Application.DTOs;
using SGFME.Application.Models;
using SGFME.Domain.Entidades;
using SGFME.Domain.Interfaces;
using SGFME.Infrastructure.Data.Context;
using SGFME.Service.Validators;
using System;

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

        //Adicionar método para executar comando e retornar IActionResult
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
        public async Task<ActionResult<List<Paciente>>> Create(PacienteCreateDTO request)
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
                        // Inicializa a lista de contatos
                        contato = new List<Contato>(),
                        endereco = new List<Endereco>()
                    };
                    // Verifica se foi fornecido um CNS no DTO
                    if (!string.IsNullOrEmpty(request.cnsNumero))
                    {
                        // Verifica se o CNS já existe no banco de dados
                        var cnsExistente = await _context.cns.FirstOrDefaultAsync(c => c.numero == request.cnsNumero);
                        if (cnsExistente != null)
                        {
                            // Associa o CNS existente ao paciente
                            novoPaciente.cns = cnsExistente;
                        }
                        else
                        {
                            // Se o CNS não existe, cria um novo CNS
                            var novoCns = new Cns { numero = request.cnsNumero };
                            novoPaciente.cns = novoCns;
                        }
                    }
                    // Verifica se foram fornecidos detalhes do RG no DTO
                    if (!string.IsNullOrEmpty(request.rgNumero))
                    {
                        // Cria um novo objeto RG
                        var novoRg = new Rg
                        {
                            numero = request.rgNumero,
                            dataEmissao = request.rgDataEmissao,
                            orgaoExpedidor = request.rgOrgaoExpedidor,
                            ufEmissao = request.rgUfEmissao
                        };

                        // Associa o RG ao paciente
                        novoPaciente.rg = novoRg;
                    }




                    foreach (var contatoDto in request.contato)
                    {
                        var contato = new Contato
                        {
                            
                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato, // Define o tipo de contato
                            paciente = novoPaciente // Define a relação com o paciente
                        };
                        novoPaciente.contato.Add(contato);
                    }

                    // Adiciona os endereços ao paciente
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
                            idTipoEndereco = enderecoDto.idTipoEndereco, // Define o tipo de contato
                            paciente = novoPaciente // Define a relação com o paciente
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
                    // Log the exception (consider using a logging framework)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao criar paciente.");
                }
            }
        }


        //EndPoint para alterar um paciente:
        [HttpPut]
        public IActionResult Update(PacienteModel pacienteModel)
        {
            if (pacienteModel == null)
                return NotFound();
            return Execute(() => _baseService.Update<PacienteModel, PacienteValidator>(pacienteModel));
        }

        //EndPoint para excluir um paciente:
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id == 0)
                return NotFound();
            Execute(() =>
            {
                _baseService.Delete(id);
                return true;
            });
            return new NoContentResult();
        }

        // EndPoint para selecionar um paciente pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<PacienteModel>(id));
        }
    }
}
