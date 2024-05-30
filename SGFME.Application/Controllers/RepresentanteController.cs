using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGFME.Application.DTOs;
using SGFME.Application.Models;
using SGFME.Domain.Entidades;
using SGFME.Domain.Interfaces;
using SGFME.Infrastructure.Data.Context;
using SGFME.Service.Validators;

namespace SGFME.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepresentanteController : ControllerBase
    {
        private IBaseService<Representante> _baseService;

        public RepresentanteController(IBaseService<Representante> baseService)
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

        //EndPoint para criar um Representante:
        [HttpPost]
        public IActionResult Create(RepresentanteModel Representante)
        {
            if (Representante == null)
                return NotFound();
            return Execute(() => _baseService.Add<RepresentanteModel,
           RepresentanteValidator>(Representante));
        }

        //EndPoint para alterar um Representante:
        [HttpPut]
        public IActionResult Update(RepresentanteModel Representante)
        {
            if (Representante == null)
                return NotFound();
            return Execute(() => _baseService.Update<RepresentanteModel,
           RepresentanteValidator>(Representante));
        }


        //EndPoint para excluir um Representante:
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


        // EndPoint para selecionar um Representante pelo ID:
        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById<RepresentanteModel>(id));
        }
    }

}

