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
    public class NaturalidadeController : ControllerBase
    {
        private IBaseService<Naturalidade> _baseService;

        public NaturalidadeController(IBaseService<Naturalidade> baseService)
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

        //EndPoint para criar um Naturalidade:
        [HttpPost]
        public IActionResult Create(NaturalidadeModel Naturalidade)
        {
            if (Naturalidade == null)
                return NotFound();
            return Execute(() => _baseService.Add<NaturalidadeModel,
           NaturalidadeValidator>(Naturalidade));
        }

        //EndPoint para alterar um Naturalidade:
        [HttpPut]
        public IActionResult Update(NaturalidadeModel Naturalidade)
        {
            if (Naturalidade == null)
                return NotFound();
            return Execute(() => _baseService.Update<NaturalidadeModel,
           NaturalidadeValidator>(Naturalidade));
        }


        //EndPoint para excluir um Naturalidade:
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


        // EndPoint para selecionar um Naturalidade pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<NaturalidadeModel>(id));
        }
    }
}
