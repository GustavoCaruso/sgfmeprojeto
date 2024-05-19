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
    public class CpfController : ControllerBase
    {
        private IBaseService<Cpf> _baseService;

        public CpfController(IBaseService<Cpf> baseService)
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

        //EndPoint para criar um Cpf:
        [HttpPost]
        public IActionResult Create(CpfModel Cpf)
        {
            if (Cpf == null)
                return NotFound();
            return Execute(() => _baseService.Add<CpfModel,
           CpfValidator>(Cpf));
        }

        //EndPoint para alterar um Cpf:
        [HttpPut]
        public IActionResult Update(CpfModel Cpf)
        {
            if (Cpf == null)
                return NotFound();
            return Execute(() => _baseService.Update<CpfModel,
           CpfValidator>(Cpf));
        }


        //EndPoint para excluir um Cpf:
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


        // EndPoint para selecionar um Cpf pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<CpfModel>(id));
        }
    }
}
