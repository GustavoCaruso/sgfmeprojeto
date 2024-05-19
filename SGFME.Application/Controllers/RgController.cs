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
    public class RgController : ControllerBase
    {
        private IBaseService<Rg> _baseService;

        public RgController(IBaseService<Rg> baseService)
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

        //EndPoint para criar um Rg:
        [HttpPost]
        public IActionResult Create(RgModel Rg)
        {
            if (Rg == null)
                return NotFound();
            return Execute(() => _baseService.Add<RgModel,
           RgValidator>(Rg));
        }

        //EndPoint para alterar um Rg:
        [HttpPut]
        public IActionResult Update(RgModel Rg)
        {
            if (Rg == null)
                return NotFound();
            return Execute(() => _baseService.Update<RgModel,
           RgValidator>(Rg));
        }


        //EndPoint para excluir um Rg:
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


        // EndPoint para selecionar um Rg pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<RgModel>(id));
        }
    }
}
