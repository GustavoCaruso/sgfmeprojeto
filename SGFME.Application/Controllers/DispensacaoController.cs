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

        [HttpPost]
        public async Task<ActionResult<Dispensacao>> Create(DispensacaoDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Criando a nova Dispensacao
                    var novaDispensacao = new Dispensacao
                    {
                        idPaciente = request.idPaciente,
                        idCid = request.idCid,
                        inicioApac = request.inicioApac,
                        fimApac = request.fimApac,
                        observacao = request.observacao,
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

                    // Salvando a Dispensacao
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
    }
}
