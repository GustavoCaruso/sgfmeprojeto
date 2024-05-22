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
    public class EstabelecimentoSaudeController : ControllerBase
    {
        private IBaseService<EstabelecimentoSaude> _baseService;
        private readonly SqlServerContext _context;

        public EstabelecimentoSaudeController(IBaseService<EstabelecimentoSaude> baseService, SqlServerContext context)
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

        //EndPoint para criar um EstabelecimentoSaude:
        [HttpPost]
        public async Task<ActionResult<List<EstabelecimentoSaude>>> Create(EstabelecimentoSaudeDTO request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var novoEstabelecimentoSaude = new EstabelecimentoSaude
                    {
                        id = request.id,
                        nomeFantasia = request.nomeFantasia,
                        razaoSocial = request.razaoSocial,
                        cnes = request.cnes,
                        // Inicializa a lista de contatos
                        contato = new List<Contato>()
                    };
                    foreach (var contatoDto in request.contato)
                    {
                        var contato = new Contato
                        {

                            valor = contatoDto.valor,
                            idTipoContato = contatoDto.idTipoContato, // Define o tipo de contato
                            estabelecimentosaude = novoEstabelecimentoSaude, // Define a relação com o Estabeleciment Saúde
                            discriminator = "EstabelecimentoSaude", // Define o discriminador
                        };
                        novoEstabelecimentoSaude.contato.Add(contato);
                    }

                    _context.estabelecimentosaude.Add(novoEstabelecimentoSaude);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var createdEstabelecimentoSaude = await _context.estabelecimentosaude
                        .Include(p => p.contato)
                        .FirstOrDefaultAsync(e => e.id == novoEstabelecimentoSaude.id);

                    return CreatedAtAction(nameof(Create), new { id = createdEstabelecimentoSaude.id }, createdEstabelecimentoSaude);

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log the exception (consider using a logging framework)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao criar Estabelecimento de Saúde.");
                }
            }
        }


        //EndPoint para alterar um EstabelecimentoSaude:
        [HttpPut]
        public IActionResult Update(EstabelecimentoSaudeModel EstabelecimentoSaude)
        {
            if (EstabelecimentoSaude == null)
                return NotFound();
            return Execute(() => _baseService.Update<EstabelecimentoSaudeModel,
           EstabelecimentoSaudeValidator>(EstabelecimentoSaude));
        }


        //EndPoint para excluir um EstabelecimentoSaude:
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

        [HttpGet]
        [Route("api/TipoContato")]
        public IActionResult ObterTiposContato()
        {
            try
            {
                var tiposContato = _context.tipocontato.ToList();
                return Ok(tiposContato);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult selecionarTodos()
        {
            //select * from produtos
            return Execute(() => _baseService.Get<EstabelecimentoSaudeModel>());
        }

        // EndPoint para selecionar um EstabelecimentoSaude pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<EstabelecimentoSaudeModel>(id));
        }
    }
}
