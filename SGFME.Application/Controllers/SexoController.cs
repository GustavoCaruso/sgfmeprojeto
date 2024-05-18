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
    public class SexoController : ControllerBase
    {
        private IBaseService<Sexo> _baseService;

        public SexoController(IBaseService<Sexo> baseService)
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

        //EndPoint para criar um Sexo:
        [HttpPost]
        public IActionResult Create(SexoModel Sexo)
        {
            if (Sexo == null)
                return NotFound();
            return Execute(() => _baseService.Add<SexoModel,
           SexoValidator>(Sexo));
        }

        //EndPoint para alterar um Sexo:
        [HttpPut]
        public IActionResult Update(SexoModel Sexo)
        {
            if (Sexo == null)
                return NotFound();
            return Execute(() => _baseService.Update<SexoModel,
           SexoValidator>(Sexo));
        }


        //EndPoint para excluir um Sexo:
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


        // EndPoint para selecionar um Sexo pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<SexoModel>(id));
        }
    }
}
