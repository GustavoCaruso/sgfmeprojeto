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
    public class MedicamentoController : ControllerBase
    {
        private IBaseService<Medicamento> _baseService;

        public MedicamentoController(IBaseService<Medicamento> baseService)
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

        //EndPoint para criar um Medicamento:
        [HttpPost]
        public IActionResult Create(MedicamentoModel Medicamento)
        {
            if (Medicamento == null)
                return NotFound();
            return Execute(() => _baseService.Add<MedicamentoModel,
           MedicamentoValidator>(Medicamento));
        }

        //EndPoint para alterar um Medicamento:
        [HttpPut]
        public IActionResult Update(MedicamentoModel Medicamento)
        {
            if (Medicamento == null)
                return NotFound();
            return Execute(() => _baseService.Update<MedicamentoModel,
           MedicamentoValidator>(Medicamento));
        }


        //EndPoint para excluir um Medicamento:
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


        // EndPoint para selecionar um Medicamento pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<MedicamentoModel>(id));
        }
    }


}

