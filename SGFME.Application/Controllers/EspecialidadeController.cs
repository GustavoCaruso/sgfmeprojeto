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
    public class EspecialidadeController : ControllerBase
    {
        private IBaseService<Especialidade> _baseService;

        public EspecialidadeController(IBaseService<Especialidade> baseService)
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

        //EndPoint para criar um Especialidade:
        [HttpPost]
        public IActionResult Create(EspecialidadeModel Especialidade)
        {
            if (Especialidade == null)
                return NotFound();
            return Execute(() => _baseService.Add<EspecialidadeModel,
           EspecialidadeValidator>(Especialidade));
        }

        //EndPoint para alterar um Especialidade:
        [HttpPut]
        public IActionResult Update(EspecialidadeModel Especialidade)
        {
            if (Especialidade == null)
                return NotFound();
            return Execute(() => _baseService.Update<EspecialidadeModel,
           EspecialidadeValidator>(Especialidade));
        }


        //EndPoint para excluir um Especialidade:
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


        // EndPoint para selecionar um Especialidade pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<EspecialidadeModel>(id));
        }
    }
}
