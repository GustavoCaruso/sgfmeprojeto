using SGFME.Domain.Entidades;
using System.Text.Json.Serialization;

namespace SGFME.Application.Models
{
    public class PacienteModel
    {
        public  virtual long id { get; set; }
        public  string nomeCompleto { get; set; }
        
        public decimal peso { get; set; }
        public decimal altura { get; set; }
        
        public DateTime dataNascimento { get; set; }
        public int idade { get; set; }
        
        public  string nomeMae { get; set; }


        //relacionamento de one to one
        public long idCns { get; set; }
        public virtual Cns cns { get; set; }

        //relacionamento de one to one
        public long idRg { get; set; }
        public virtual Rg rg { get; set; }


        public virtual ICollection<Contato> contato { get; set; } = new List<Contato>();
        public virtual ICollection<Endereco> endereco { get; set; } = new List<Endereco>();
    }
}
