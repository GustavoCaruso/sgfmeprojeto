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
    public class PerfilUsuarioController : ControllerBase
    {
        private IBaseService<PerfilUsuario> _baseService;

        public PerfilUsuarioController(IBaseService<PerfilUsuario> baseService)
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

        //EndPoint para criar um PerfilUsuario:
        [HttpPost]
        public IActionResult Create(PerfilUsuarioModel PerfilUsuario)
        {
            if (PerfilUsuario == null)
                return NotFound();
            return Execute(() => _baseService.Add<PerfilUsuarioModel,
           PerfilUsuarioValidator>(PerfilUsuario));
        }

        //EndPoint para alterar um PerfilUsuario:
        [HttpPut]
        public IActionResult Update(PerfilUsuarioModel PerfilUsuario)
        {
            if (PerfilUsuario == null)
                return NotFound();
            return Execute(() => _baseService.Update<PerfilUsuarioModel,
           PerfilUsuarioValidator>(PerfilUsuario));
        }


        //EndPoint para excluir um PerfilUsuario:
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


        // EndPoint para selecionar um PerfilUsuario pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<PerfilUsuarioModel>(id));
        }
    }
}
