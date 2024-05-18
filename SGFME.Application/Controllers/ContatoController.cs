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
    public class ContatoController : ControllerBase
    {
        private IBaseService<Contato> _baseService;

        public ContatoController(IBaseService<Contato> baseService)
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

        //EndPoint para criar um Contato:
        [HttpPost]
        public IActionResult Create(ContatoModel Contato)
        {
            if (Contato == null)
                return NotFound();
            return Execute(() => _baseService.Add<ContatoModel,
           ContatoValidator>(Contato));
        }

        //EndPoint para alterar um Contato:
        [HttpPut]
        public IActionResult Update(ContatoModel Contato)
        {
            if (Contato == null)
                return NotFound();
            return Execute(() => _baseService.Update<ContatoModel,
           ContatoValidator>(Contato));
        }


        //EndPoint para excluir um Contato:
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


        // EndPoint para selecionar um Contato pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<ContatoModel>(id));
        }
    }
}
