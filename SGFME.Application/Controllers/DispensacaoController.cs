using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGFME.Domain.Entidades;
using SGFME.Domain.Interfaces;
using SGFME.Infrastructure.Data.Context;

namespace SGFME.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DispensacaoController : ControllerBase
    {
        private IBaseService<Dispensacao> _baseService;
        private readonly SqlServerContext _context;
    }
}
