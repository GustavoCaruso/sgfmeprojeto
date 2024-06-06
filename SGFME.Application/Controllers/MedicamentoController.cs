using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGFME.Application.DTOs;
using SGFME.Application.Models;
using SGFME.Domain.Entidades;
using SGFME.Domain.Interfaces;
using SGFME.Infrastructure.Data.Context;
using SGFME.Service.Validators;

namespace SGFME.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicamentoController : ControllerBase
    {
        private IBaseService<Medicamento> _baseService;
        private readonly SqlServerContext _context;

        public MedicamentoController(IBaseService<Medicamento> baseService, SqlServerContext context)
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

        //EndPoint para criar um Medicamento:
        [HttpPost]
        public async Task<ActionResult<Medicamento>> Create(MedicamentoDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    var novoMedicamento = new Medicamento
                    {
                        id = request.id,
                        nome = request.nome,
                        idStatus = request.idStatus
                        
                    };



                    // Validar a entrada usando FluentValidation
                    var validator = new MedicamentoValidator();
                    var validationResult = await validator.ValidateAsync(novoMedicamento);

                    if (!validationResult.IsValid)
                    {
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.medicamento.Add(novoMedicamento);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var createdMedicamento = await _context.medicamento

                        .Include(p => p.status)
                        

                        .FirstOrDefaultAsync(e => e.id == novoMedicamento.id);

                    return CreatedAtAction(nameof(Create), new { id = createdMedicamento.id }, createdMedicamento);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao criar Medicamento.");
                }
            }
        }

        [HttpGet("todosMedicamentos")]
        public async Task<ActionResult<List<Medicamento>>> GetAllMedicamentos()
        {
            try
            {
                var medicamentos = await _context.medicamento

                    .Include(m => m.status)
                    

                    .ToListAsync();

                return Ok(medicamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os medicamentos.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, MedicamentoDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var medicamento = await _context.medicamento
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (medicamento == null)
                    {
                        return NotFound("medicamento não encontrado.");
                    }

                    medicamento.nome = request.nome;
                   

                    medicamento.idStatus = request.idStatus; // Associação com Status
                   






                    // Validar a entrada usando FluentValidation
                    var validator = new MedicamentoValidator();
                    var validationResult = await validator.ValidateAsync(medicamento);

                    if (!validationResult.IsValid)
                    {
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.medicamento.Update(medicamento);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(medicamento);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar medicamento.");
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
                    var medicamentoExistente = await _context.medicamento

                        .FirstOrDefaultAsync(m => m.id == id);

                    if (medicamentoExistente == null)
                    {
                        return NotFound("medicamento não encontrado.");
                    }


                    _context.medicamento.Remove(medicamentoExistente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("medicamento excluído com sucesso.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir medicamento.");
                }
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetmedicamentoById(int id)
        {
            try
            {
                var medicamento = await _context.medicamento

                    .Include(m => m.status) // Incluir status
                    

                    .FirstOrDefaultAsync(m => m.id == id);

                if (medicamento == null)
                {
                    return NotFound("medicamento não encontrado.");
                }

                return Ok(medicamento);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar o medicamento.");
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
            return Execute(() => _baseService.Get<MedicamentoModel>());
        }
    }


}

