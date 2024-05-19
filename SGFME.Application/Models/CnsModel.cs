using SGFME.Domain.Entidades;

namespace SGFME.Application.Models
{
    public class CnsModel
    {
        public long id { get; set; }
        public string numero { get; set; }

        public virtual Paciente paciente { get; set; } // Navegação de volta para Paciente
    }
}
