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
    public class TipoProcessoController : ControllerBase
    {
        private IBaseService<TipoProcesso> _baseService;

        public TipoProcessoController(IBaseService<TipoProcesso> baseService)
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

        //EndPoint para criar um Status:
        [HttpPost]
        public IActionResult Create(TipoProcessoModel tipoprocesso)
        {
            if (tipoprocesso == null)
                return NotFound();
            return Execute(() => _baseService.Add<TipoProcessoModel,
           TipoProcessoValidator>(tipoprocesso));
        }

        //EndPoint para alterar um Status:
        [HttpPut]
        public IActionResult Update(TipoProcessoModel tipoprocesso)
        {
            if (tipoprocesso == null)
                return NotFound();
            return Execute(() => _baseService.Update<TipoProcessoModel,
           TipoProcessoValidator>(tipoprocesso));
        }


        //EndPoint para excluir um Status:
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


        // EndPoint para selecionar um Status pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<TipoProcessoModel>(id));
        }
    }
}
