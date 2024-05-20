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
    public class PessoaController : ControllerBase
    {
        private IBaseService<Pessoa> _baseService;

        public PessoaController(IBaseService<Pessoa> baseService)
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

        //EndPoint para criar um Pessoa:
        [HttpPost]
        public IActionResult Create(PessoaModel Pessoa)
        {
            if (Pessoa == null)
                return NotFound();
            return Execute(() => _baseService.Add<PessoaModel,
           PessoaValidator>(Pessoa));
        }

        //EndPoint para alterar um Pessoa:
        [HttpPut]
        public IActionResult Update(PessoaModel Pessoa)
        {
            if (Pessoa == null)
                return NotFound();
            return Execute(() => _baseService.Update<PessoaModel,
           PessoaValidator>(Pessoa));
        }


        //EndPoint para excluir um Pessoa:
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


        // EndPoint para selecionar um Pessoa pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<PessoaModel>(id));
        }
    }
}
