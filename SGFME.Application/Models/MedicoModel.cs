using SGFME.Domain.Entidades;

namespace SGFME.Application.Models
{
    public class MedicoModel
    {
        public long id { get; set; }
        public String nomeCompleto { get; set; }
        public DateTime dataNascimento { get; set; }
        public String crm { get; set; }
        public virtual ICollection<Contato> contato { get; set; } = new List<Contato>();
        
    }
}
