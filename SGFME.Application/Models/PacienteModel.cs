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


        //propriedades adicionadas agora


        //propriedades do rg
        public string rgNumero { get; set; }
        public DateTime rgDataEmissao { get; set; }
        public string rgOrgaoExpedidor { get; set; }
        public string rgUfEmissao { get; set; }


        //propriedades do cns
        public string cnsNumero { get; set; }

        //propriedades do cpf
        public string cpfNumero { get; set; }


        public string nomeConjuge { get; set; }
        public DateTime dataCadastro { get; set; }


        public virtual ICollection<Contato> contato { get; set; } = new List<Contato>();
        public virtual ICollection<Endereco> endereco { get; set; } = new List<Endereco>();
    }
}
