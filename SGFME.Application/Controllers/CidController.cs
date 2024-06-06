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
    public class CidController : ControllerBase
    {
        private IBaseService<Cid> _baseService;
        private readonly SqlServerContext _context;

        public CidController(IBaseService<Cid> baseService, SqlServerContext context)
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

        //EndPoint para criar um Cid:
        [HttpPost]
        public async Task<ActionResult<Cid>> Create(CidDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    var novoCid = new Cid
                    {
                        id = request.id,
                        codigo = request.codigo,
                        descricao = request.descricao,
                        idStatus = request.idStatus,
                        idVersaoCid = request.idVersaoCid
                    };



                    // Validar a entrada usando FluentValidation
                    var validator = new CidValidator();
                    var validationResult = await validator.ValidateAsync(novoCid);

                    if (!validationResult.IsValid)
                    {
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.cid.Add(novoCid);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var createdCid = await _context.cid
                        
                        .Include(p => p.status)
                        .Include(p => p.versaocid)

                        .FirstOrDefaultAsync(e => e.id == novoCid.id);

                    return CreatedAtAction(nameof(Create), new { id = createdCid.id }, createdCid);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao criar Cid.");
                }
            }
        }

        [HttpGet("todosCids")]
        public async Task<ActionResult<List<Cid>>> GetAllCids()
        {
            try
            {
                var cids = await _context.cid
                    
                    .Include(m => m.status)
                    .Include(m => m.versaocid)
                    
                    .ToListAsync();

                return Ok(cids);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar os cids.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, CidDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var cid = await _context.cid
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (cid == null)
                    {
                        return NotFound("cid não encontrado.");
                    }

                    cid.descricao = request.descricao;
                    cid.codigo = request.codigo;

                    cid.idStatus = request.idStatus; // Associação com Status
                    cid.idVersaoCid = request.idVersaoCid;
                    

                    

                    

                    // Validar a entrada usando FluentValidation
                    var validator = new CidValidator();
                    var validationResult = await validator.ValidateAsync(cid);

                    if (!validationResult.IsValid)
                    {
                        return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                    }

                    _context.cid.Update(cid);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(cid);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar cid.");
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
                    var cidExistente = await _context.cid
                        
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (cidExistente == null)
                    {
                        return NotFound("Cid não encontrado.");
                    }

                    
                    _context.cid.Remove(cidExistente);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("Cid excluído com sucesso.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir Cid.");
                }
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCidById(int id)
        {
            try
            {
                var cid = await _context.cid
                    
                    .Include(m => m.status) // Incluir status
                    .Include(m => m.versaocid) // Incluir status
                    
                    .FirstOrDefaultAsync(m => m.id == id);

                if (cid == null)
                {
                    return NotFound("Cid não encontrado.");
                }

                return Ok(cid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao buscar o Cid.");
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





        [HttpGet("tipoVersaoCid")]
        public IActionResult ObterTiposVersaoCid()
        {
            try
            {
                var tiposVersaoCid = _context.versaocid.ToList();
                return Ok(tiposVersaoCid);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }




        [HttpGet]
        public IActionResult SelecionarTodos()
        {
            return Execute(() => _baseService.Get<CidModel>());
        }
    }
}
