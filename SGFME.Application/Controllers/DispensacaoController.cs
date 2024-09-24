using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using SGFME.Application.Models;
using SGFME.Domain.Entidades;
using SGFME.Infrastructure.Data.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SGFME.Application.DTOs;

namespace SGFME.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DispensacaoController(SqlServerContext _context, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DispensacaoModel>>> Get()
        {
            var dispensacoes = await _context.dispensacao
                .Include(d => d.paciente)
                .Include(d => d.cid)
                .Include(d => d.Medicamentos)
                .ThenInclude(x => x.Medicamento)
                .ToListAsync();

            return _mapper.Map<List<DispensacaoModel>>(dispensacoes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DispensacaoModel>> GetAll(long id)
        {
            var dispensacao = await _context.dispensacao
                .Include(d => d.paciente)
                .Include(d => d.cid)
                .Include(d => d.Medicamentos)
                .ThenInclude(d => d.Medicamento)
                .FirstOrDefaultAsync(d => d.id == id);

            if (dispensacao == null)
            {
                return NotFound();
            }

            return _mapper.Map<DispensacaoModel>(dispensacao);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, AlterarDispensacaoDTO request)
        {
            if (id != request.Id)
            {
                return BadRequest("Id da URL não corresponde.");
            }

            var dispensacao = await _context.dispensacao
                .AsNoTracking()
                .Include(d => d.paciente)
                .Include(d => d.cid)
                .Include(d => d.Medicamentos)
                .ThenInclude(d => d.Medicamento)
                .FirstOrDefaultAsync(d => d.id == id);

            if (dispensacao == null)
            {
                return NotFound("Dispensacao não encontrada.");
            }

            dispensacao.Atualizar(request.IdPaciente,
                                  request.IdCid,
                                  request.InicioApac,
                                  request.Observacao,
                                  request.ToEntity(request.Medicamentos));

            try
            {
                _context.Update(dispensacao);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DispensacaoExists(id))
                {
                    return NotFound("Dispensacao não existe.");
                }
                else
                {
                    return StatusCode(500, "Internal server error");
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<DispensacaoModel>> Create(CriarDispensacaoDTO request)
        {
            try
            {
                var dispensacao = _mapper.Map<Dispensacao>(request);
                dispensacao.fimApac = dispensacao.inicioApac.AddMonths(6);
                _context.dispensacao.Add(dispensacao);
                await _context.SaveChangesAsync();

                var response = _mapper.Map<DispensacaoModel>(dispensacao);

                return CreatedAtAction(nameof(GetAll), new { id = dispensacao.id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var dispensacao = await _context.dispensacao.FindAsync(id);
                if (dispensacao == null)
                {
                    return NotFound();
                }

                _context.dispensacao.Remove(dispensacao);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        private bool DispensacaoExists(long id)
        {
            return _context.dispensacao.Any(e => e.id == id);
        }
    }
}