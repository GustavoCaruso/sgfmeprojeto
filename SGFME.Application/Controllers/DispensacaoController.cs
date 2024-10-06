using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGFME.Application.DTOs;
using SGFME.Domain.Entidades;
using SGFME.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGFME.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DispensacaoController : ControllerBase
    {
        private readonly SqlServerContext _context;

        public DispensacaoController(SqlServerContext context)
        {
            _context = context;
        }

        // Método para criar uma nova dispensação
        [HttpPost]
        public async Task<ActionResult<Dispensacao>> Create(DispensacaoDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var novaDispensacao = new Dispensacao
                    {
                        idPaciente = request.idPaciente,
                        idCid = request.idCid,
                        inicioApac = request.inicioApac,
                        fimApac = request.fimApac,
                        observacao = request.observacao,
                        dataRenovacao = request.dataRenovacao,
                        dataSuspensao = request.dataSuspensao,
                        idStatusProcesso = request.idStatusProcesso,
                        idTipoProcesso = request.idTipoProcesso,
                        medicamento = new List<DispensacaoMedicamento>()
                    };

                    // Adicionando medicamentos
                    foreach (var medicamentoDto in request.medicamento)
                    {
                        var medicamento = new DispensacaoMedicamento
                        {
                            idMedicamento = medicamentoDto.idMedicamento,
                            quantidade = medicamentoDto.quantidade,
                            recibo = medicamentoDto.recibo,
                            receita = medicamentoDto.receita,
                            medicamentoChegou = medicamentoDto.medicamentoChegou,
                            medicamentoEntregue = medicamentoDto.medicamentoEntregue,
                            dataEntrega = medicamentoDto.dataEntrega
                        };

                        novaDispensacao.medicamento.Add(medicamento);
                    }

                    _context.dispensacao.Add(novaDispensacao);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return CreatedAtAction(nameof(Create), new { id = novaDispensacao.id }, novaDispensacao);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Erro inesperado: {ex.Message}");
                }
            }
        }

        // Método para buscar dados básicos das dispensações
        [HttpGet("dadosBasicos")]
        public async Task<ActionResult<List<object>>> GetBasicDispensacaoData()
        {
            try
            {
                var dispensacoes = await _context.dispensacao
                    .Include(d => d.statusprocesso)       // Inclui o relacionamento com StatusProcesso
                    .Include(d => d.tipoprocesso)         // Inclui o relacionamento com TipoProcesso
                    .Include(d => d.paciente)             // Inclui o relacionamento com Paciente
                    .Include(d => d.cid)                  // Inclui o relacionamento com CID
                    .Select(d => new
                    {
                        d.id,
                        d.inicioApac,
                        d.fimApac,
                        nomePaciente = d.paciente.nomeCompleto,  // Inclui o nome do paciente
                        nomeCid = d.cid.codigo,                  // Inclui o nome do CID
                        d.idStatusProcesso,
                        d.idTipoProcesso,
                        statusNome = d.statusprocesso.nome,      // Inclui o nome do status
                        tipoProcessoNome = d.tipoprocesso.nome   // Inclui o nome do tipo de processo
                    })
                    .ToListAsync();

                return Ok(dispensacoes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os dados básicos das dispensações.");
            }
        }

        // Método para buscar medicamentos relacionados a uma dispensação pelo ID
        [HttpGet("{id}/medicamentos")]
        public async Task<ActionResult<List<DispensacaoMedicamento>>> GetMedicamentosByDispensacaoId(long id)
        {
            try
            {
                var medicamentos = await _context.dispensacaomedicamento
                    .Where(m => m.idDispensacao == id)
                    .ToListAsync();

                if (medicamentos == null || !medicamentos.Any())
                {
                    return NotFound("Medicamentos não encontrados.");
                }

                return Ok(medicamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar medicamentos.");
            }
        }

        // Método para atualizar uma dispensação
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, DispensacaoDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var dispensacao = await _context.dispensacao
                        .Include(d => d.medicamento)
                        .FirstOrDefaultAsync(d => d.id == id);

                    if (dispensacao == null)
                    {
                        return NotFound("Dispensação não encontrada.");
                    }

                    dispensacao.inicioApac = request.inicioApac;
                    dispensacao.fimApac = request.fimApac;
                    dispensacao.observacao = request.observacao;
                    dispensacao.dataRenovacao = request.dataRenovacao;
                    dispensacao.dataSuspensao = request.dataSuspensao;
                    dispensacao.idStatusProcesso = request.idStatusProcesso;
                    dispensacao.idTipoProcesso = request.idTipoProcesso;

                    // Atualizando os medicamentos
                    _context.dispensacaomedicamento.RemoveRange(dispensacao.medicamento);
                    dispensacao.medicamento.Clear();

                    foreach (var medicamentoDto in request.medicamento)
                    {
                        var medicamento = new DispensacaoMedicamento
                        {
                            idMedicamento = medicamentoDto.idMedicamento,
                            quantidade = medicamentoDto.quantidade,
                            recibo = medicamentoDto.recibo,
                            receita = medicamentoDto.receita,
                            medicamentoChegou = medicamentoDto.medicamentoChegou,
                            medicamentoEntregue = medicamentoDto.medicamentoEntregue,
                            dataEntrega = medicamentoDto.dataEntrega
                        };
                        dispensacao.medicamento.Add(medicamento);
                    }

                    _context.dispensacao.Update(dispensacao);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(dispensacao);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar a dispensação.");
                }
            }
        }

        // Método para excluir uma dispensação
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var dispensacaoExistente = await _context.dispensacao
                        .Include(d => d.medicamento)
                        .FirstOrDefaultAsync(d => d.id == id);

                    if (dispensacaoExistente == null)
                    {
                        return NotFound("Dispensação não encontrada.");
                    }

                    _context.dispensacaomedicamento.RemoveRange(dispensacaoExistente.medicamento);
                    _context.dispensacao.Remove(dispensacaoExistente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("Dispensação excluída com sucesso.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir a dispensação.");
                }
            }
        }

        // Método para buscar dados completos de uma dispensação
        [HttpGet("{id}/dadosCompletos")]
        public async Task<ActionResult<Dispensacao>> GetCompleteDispensacaoData(long id)
        {
            try
            {
                var dispensacao = await _context.dispensacao
                    .Include(d => d.medicamento)
                    .FirstOrDefaultAsync(d => d.id == id);

                if (dispensacao == null)
                {
                    return NotFound("Dispensação não encontrada.");
                }

                return Ok(dispensacao);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os dados completos da dispensação.");
            }
        }


        [HttpPut("{id}/medicamentos")]
        public async Task<IActionResult> UpdateMedicamentos(long id, List<DispensacaoMedicamentoDTO> novosMedicamentos)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var dispensacao = await _context.dispensacao
                        .Include(d => d.medicamento) // Certifique-se de que "medicamentos" é o nome correto da propriedade de navegação
                        .FirstOrDefaultAsync(d => d.id == id);

                    if (dispensacao == null)
                    {
                        return NotFound("Dispensação não encontrada.");
                    }

                    // Validação dos novos medicamentos
                    foreach (var medicamentoDto in novosMedicamentos)
                    {
                        if (!_context.medicamento.Any(m => m.id == medicamentoDto.idMedicamento))
                        {
                            return BadRequest($"Medicamento com id {medicamentoDto.idMedicamento} não encontrado.");
                        }
                    }

                    // Remove os medicamentos antigos
                    _context.dispensacaomedicamento.RemoveRange(dispensacao.medicamento); // Use o nome correto do DbSet
                    dispensacao.medicamento.Clear();

                    // Adiciona os novos medicamentos
                    foreach (var medicamentoDto in novosMedicamentos)
                    {
                        var medicamento = new DispensacaoMedicamento
                        {
                            idMedicamento = medicamentoDto.idMedicamento,
                            quantidade = medicamentoDto.quantidade,
                            dataEntrega = medicamentoDto.dataEntrega,
                            recibo = medicamentoDto.recibo,
                            receita = medicamentoDto.receita,
                            idDispensacao = dispensacao.id  // Relaciona com o ID da dispensação
                        };
                        dispensacao.medicamento.Add(medicamento);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(dispensacao.medicamento);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar medicamentos.");
                }
            }
        }





    }
}
