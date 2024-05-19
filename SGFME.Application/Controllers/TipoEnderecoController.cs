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
    public class TipoEnderecoController : ControllerBase
    {
        private IBaseService<TipoEndereco> _baseService;

        public TipoEnderecoController(IBaseService<TipoEndereco> baseService)
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

        //EndPoint para criar um TipoEndereco:
        [HttpPost]
        public IActionResult Create(TipoEnderecoModel TipoEndereco)
        {
            if (TipoEndereco == null)
                return NotFound();
            return Execute(() => _baseService.Add<TipoEnderecoModel,
           TipoEnderecoValidator>(TipoEndereco));
        }

        //EndPoint para alterar um TipoEndereco:
        [HttpPut]
        public IActionResult Update(TipoEnderecoModel TipoEndereco)
        {
            if (TipoEndereco == null)
                return NotFound();
            return Execute(() => _baseService.Update<TipoEnderecoModel,
           TipoEnderecoValidator>(TipoEndereco));
        }


        //EndPoint para excluir um TipoEndereco:
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


        // EndPoint para selecionar um TipoEndereco pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<TipoEnderecoModel>(id));
        }
    }
}
