using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGFME.Application.Models;
using SGFME.Domain.Entidades;
using SGFME.Domain.Interfaces;
using SGFME.Infrastructure.Data.Context;
using SGFME.Service.Validators;

namespace SGFME.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusProcessoController : ControllerBase
    {
        private IBaseService<StatusProcesso> _baseService;
        private readonly SqlServerContext _context;

        public StatusProcessoController(IBaseService<StatusProcesso> baseService, SqlServerContext context)
        {
            _baseService = baseService;
            _context = context;
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
        public IActionResult Create(StatusProcessoModel statusprocesso)
        {
            if (statusprocesso == null)
                return NotFound();
            return Execute(() => _baseService.Add<StatusProcessoModel,
           StatusProcessoValidator>(statusprocesso));
        }

        //EndPoint para alterar um Status:
        [HttpPut]
        public IActionResult Update(StatusProcessoModel statusprocesso)
        {
            if (statusprocesso == null)
                return NotFound();
            return Execute(() => _baseService.Update<StatusProcessoModel,
           StatusProcessoValidator>(statusprocesso));
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

            return Execute(() => _baseService.GetById<StatusProcessoModel>(id));
        }

        [HttpGet]
        public IActionResult SelecionarTodos()
        {
            return Execute(() => _baseService.Get<StatusProcessoModel>());
        }
    }
}
