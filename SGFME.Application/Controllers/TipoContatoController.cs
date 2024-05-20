using Microsoft.AspNetCore.Authorization;
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
    public class TipoContatoController : ControllerBase
    {
        private IBaseService<TipoContato> _baseService;

        public TipoContatoController(IBaseService<TipoContato> baseService)
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

        //EndPoint para criar um TipoContato:
        [HttpPost]
        public IActionResult Create(TipoContatoModel TipoContato)
        {
            if (TipoContato == null)
                return NotFound();
            return Execute(() => _baseService.Add<TipoContatoModel,
           TipoContatoValidator>(TipoContato));
        }

        //EndPoint para alterar um TipoContato:
        [HttpPut]
        public IActionResult Update(TipoContatoModel TipoContato)
        {
            if (TipoContato == null)
                return NotFound();
            return Execute(() => _baseService.Update<TipoContatoModel,
           TipoContatoValidator>(TipoContato));
        }


        //EndPoint para excluir um TipoContato:
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


        // EndPoint para selecionar um TipoContato pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<TipoContatoModel>(id));
        }

    }
}
