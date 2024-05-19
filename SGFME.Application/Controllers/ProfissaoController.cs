using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGFME.Application.Models;
using SGFME.Domain.Entidades;
using SGFME.Domain.Interfaces;
using SGFME.Service.Validators;

namespace SGFME.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfissaoController : ControllerBase
    {
        private IBaseService<Profissao> _baseService;

        public ProfissaoController(IBaseService<Profissao> baseService)
        {
            _baseService = baseService;
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

        //EndPoint para criar um Profissao:
        [HttpPost]
        public IActionResult Create(ProfissaoModel Profissao)
        {
            if (Profissao == null)
                return NotFound();
            return Execute(() => _baseService.Add<ProfissaoModel,
           ProfissaoValidator>(Profissao));
        }

        //EndPoint para alterar um Profissao:
        [HttpPut]
        public IActionResult Update(ProfissaoModel Profissao)
        {
            if (Profissao == null)
                return NotFound();
            return Execute(() => _baseService.Update<ProfissaoModel,
           ProfissaoValidator>(Profissao));
        }


        //EndPoint para excluir um Profissao:
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


        // EndPoint para selecionar um Profissao pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<ProfissaoModel>(id));
        }
    }
}
