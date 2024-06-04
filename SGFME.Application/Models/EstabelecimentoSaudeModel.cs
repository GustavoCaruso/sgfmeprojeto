using SGFME.Domain.Entidades;

namespace SGFME.Application.Models
{
    public class EstabelecimentoSaudeModel
    {
        public long id { get; set; }
        public String nomeFantasia { get; set; }
        public String razaoSocial { get; set; }
        public String cnes { get; set; }
        public long idStatus { get; set; }
        public virtual Status status { get; set; }
        public virtual ICollection<Contato> contato { get; set; } = new List<Contato>();
        public virtual ICollection<Endereco> endereco { get; set; } = new List<Endereco>();
    }
}
