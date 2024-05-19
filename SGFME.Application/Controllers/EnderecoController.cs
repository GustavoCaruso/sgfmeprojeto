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
    public class EnderecoController : ControllerBase
    {
        private IBaseService<Endereco> _baseService;

        public EnderecoController(IBaseService<Endereco> baseService)
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

        //EndPoint para criar um Endereco:
        [HttpPost]
        public IActionResult Create(EnderecoModel Endereco)
        {
            if (Endereco == null)
                return NotFound();
            return Execute(() => _baseService.Add<EnderecoModel,
           EnderecoValidator>(Endereco));
        }

        //EndPoint para alterar um Endereco:
        [HttpPut]
        public IActionResult Update(EnderecoModel Endereco)
        {
            if (Endereco == null)
                return NotFound();
            return Execute(() => _baseService.Update<EnderecoModel,
           EnderecoValidator>(Endereco));
        }


        //EndPoint para excluir um Endereco:
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


        // EndPoint para selecionar um Endereco pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<EnderecoModel>(id));
        }
    }
}
