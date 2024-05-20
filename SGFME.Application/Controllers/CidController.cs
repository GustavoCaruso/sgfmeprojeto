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
    public class CidController : ControllerBase
    {
        private IBaseService<Cid> _baseService;

        public CidController(IBaseService<Cid> baseService)
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

        //EndPoint para criar um Cid:
        [HttpPost]
        public IActionResult Create(CidModel Cid)
        {
            if (Cid == null)
                return NotFound();
            return Execute(() => _baseService.Add<CidModel,
           CidValidator>(Cid));
        }

        //EndPoint para alterar um Cid:
        [HttpPut]
        public IActionResult Update(CidModel Cid)
        {
            if (Cid == null)
                return NotFound();
            return Execute(() => _baseService.Update<CidModel,
           CidValidator>(Cid));
        }


        //EndPoint para excluir um Cid:
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


        // EndPoint para selecionar um Cid pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<CidModel>(id));
        }
    }
}
