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
    public class EstadoCivilController : ControllerBase
    {
        private IBaseService<EstadoCivil> _baseService;

        public EstadoCivilController(IBaseService<EstadoCivil> baseService)
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

        //EndPoint para criar um EstadoCivil:
        [HttpPost]
        public IActionResult Create(EstadoCivilModel EstadoCivil)
        {
            if (EstadoCivil == null)
                return NotFound();
            return Execute(() => _baseService.Add<EstadoCivilModel,
           EstadoCivilValidator>(EstadoCivil));
        }

        //EndPoint para alterar um EstadoCivil:
        [HttpPut]
        public IActionResult Update(EstadoCivilModel EstadoCivil)
        {
            if (EstadoCivil == null)
                return NotFound();
            return Execute(() => _baseService.Update<EstadoCivilModel,
           EstadoCivilValidator>(EstadoCivil));
        }


        //EndPoint para excluir um EstadoCivil:
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


        // EndPoint para selecionar um EstadoCivil pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<EstadoCivilModel>(id));
        }
    }
}
