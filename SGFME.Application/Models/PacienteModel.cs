using SGFME.Domain.Entidades;
using System.Text.Json.Serialization;

namespace SGFME.Application.Models
{
    public class PacienteModel
    {
        public  virtual long id { get; set; }
        public  string nomeCompleto { get; set; }
        public  string sexo { get; set; }
        public  string rg { get; set; }
        public  string cpf { get; set; }
        public  string cns { get; set; }
        public decimal peso { get; set; }
        public decimal altura { get; set; }
        //public String cid { get; set; } cid vai se relacionar com outra tabela
        public DateTime dataNascimento { get; set; }
        public  string naturalidade { get; set; }
        public  string ufNaturalidade { get; set; }
        public  string corRaca { get; set; }
        public  string estadoCivil { get; set; }
        //public String status { get; set; } status vai se relacionar com outra tabela
        //public String medicacao { get; set; } medicacao vai se relacionar com outra tabela
        public  string nomeMae { get; set; }

        public virtual ICollection<Contato> contato { get; set; } = new List<Contato>();
    }
}
