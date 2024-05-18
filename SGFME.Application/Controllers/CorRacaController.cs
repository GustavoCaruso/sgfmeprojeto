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
    public class CorRacaController : ControllerBase
    {
        private IBaseService<CorRaca> _baseService;

        public CorRacaController(IBaseService<CorRaca> baseService)
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

        //EndPoint para criar um CorRaca:
        [HttpPost]
        public IActionResult Create(CorRacaModel CorRaca)
        {
            if (CorRaca == null)
                return NotFound();
            return Execute(() => _baseService.Add<CorRacaModel,
           CorRacaValidator>(CorRaca));
        }

        //EndPoint para alterar um CorRaca:
        [HttpPut]
        public IActionResult Update(CorRacaModel CorRaca)
        {
            if (CorRaca == null)
                return NotFound();
            return Execute(() => _baseService.Update<CorRacaModel,
           CorRacaValidator>(CorRaca));
        }


        //EndPoint para excluir um CorRaca:
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


        // EndPoint para selecionar um CorRaca pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<CorRacaModel>(id));
        }
    }
}
