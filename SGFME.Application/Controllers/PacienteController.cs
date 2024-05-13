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
    public class PacienteController : ControllerBase
    {
        private IBaseService<Paciente> _baseService;

        public PacienteController(IBaseService<Paciente> baseService)
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

        //EndPoint para criar um paciente:
        [HttpPost]
        public IActionResult Create(PacienteModel Paciente)
        {
            if (Paciente == null)
                return NotFound();
            return Execute(() => _baseService.Add<PacienteModel,
           PacienteValidator>(Paciente));
        }

        //EndPoint para alterar um paciente:
        [HttpPut]
        public IActionResult Update(PacienteModel Paciente)
        {
            if (Paciente == null)
                return NotFound();
            return Execute(() => _baseService.Update<PacienteModel,
           PacienteValidator>(Paciente));
        }


        //EndPoint para excluir um paciente:
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


        // EndPoint para selecionar um paciente pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<PacienteModel>(id));
        }


    }
}
