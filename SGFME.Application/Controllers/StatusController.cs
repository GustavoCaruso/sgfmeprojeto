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
    public class StatusController : ControllerBase
    {
        private IBaseService<Status> _baseService;

        public StatusController(IBaseService<Status> baseService)
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
        public IActionResult Create(StatusModel Status)
        {
            if (Status == null)
                return NotFound();
            return Execute(() => _baseService.Add<StatusModel,
           StatusValidator>(Status));
        }

        //EndPoint para alterar um Status:
        [HttpPut]
        public IActionResult Update(StatusModel Status)
        {
            if (Status == null)
                return NotFound();
            return Execute(() => _baseService.Update<StatusModel,
           StatusValidator>(Status));
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

            return Execute(() => _baseService.GetById<StatusModel>(id));
        }
    }
}
