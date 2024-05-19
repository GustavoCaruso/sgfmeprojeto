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
    public class CnsController : ControllerBase
    {
        private IBaseService<Cns> _baseService;

        public CnsController(IBaseService<Cns> baseService)
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

        //EndPoint para criar um Cns:
        [HttpPost]
        public IActionResult Create(CnsModel Cns)
        {
            if (Cns == null)
                return NotFound();
            return Execute(() => _baseService.Add<CnsModel,
           CnsValidator>(Cns));
        }

        //EndPoint para alterar um Cns:
        [HttpPut]
        public IActionResult Update(CnsModel Cns)
        {
            if (Cns == null)
                return NotFound();
            return Execute(() => _baseService.Update<CnsModel,
           CnsValidator>(Cns));
        }


        //EndPoint para excluir um Cns:
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


        // EndPoint para selecionar um Cns pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<CnsModel>(id));
        }
    }
}
