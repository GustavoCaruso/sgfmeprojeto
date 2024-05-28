using SGFME.Domain.Entidades;

namespace SGFME.Application.DTOs
{
    public class MedicoDTO
    {
        public long id { get; set; }
        public String nomeCompleto { get; set; }
        public DateTime dataNascimento { get; set; }
        public String crm { get; set; }
        public List<ContatoCreateDTO> contato { get; set; } = new List<ContatoCreateDTO>();
        
    }
}
