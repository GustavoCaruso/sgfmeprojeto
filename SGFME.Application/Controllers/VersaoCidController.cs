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
    public class VersaoCidController : ControllerBase
    {
        private IBaseService<VersaoCid> _baseService;

        public VersaoCidController(IBaseService<VersaoCid> baseService)
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

        //EndPoint para criar um VersaoCid:
        [HttpPost]
        public IActionResult Create(VersaoCidModel VersaoCid)
        {
            if (VersaoCid == null)
                return NotFound();
            return Execute(() => _baseService.Add<VersaoCidModel,
           VersaoCidValidator>(VersaoCid));
        }

        //EndPoint para alterar um VersaoCid:
        [HttpPut]
        public IActionResult Update(VersaoCidModel VersaoCid)
        {
            if (VersaoCid == null)
                return NotFound();
            return Execute(() => _baseService.Update<VersaoCidModel,
           VersaoCidValidator>(VersaoCid));
        }


        //EndPoint para excluir um VersaoCid:
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


        // EndPoint para selecionar um VersaoCid pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<VersaoCidModel>(id));
        }
    }
}
