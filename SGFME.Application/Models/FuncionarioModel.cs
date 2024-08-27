using SGFME.Domain.Entidades;
using System.Text.Json.Serialization;

namespace SGFME.Application.Models
{
    public class FuncionarioModel
    {
        public virtual long id { get; set; }
        public string nomeCompleto { get; set; }
        public DateTime dataNascimento { get; set; }
        public DateTime dataCadastro { get; set; }
        public string nomeMae { get; set; }
        public string rgNumero { get; set; }
        public DateTime rgDataEmissao { get; set; }
        public string rgOrgaoExpedidor { get; set; }
        public string rgUfEmissao { get; set; }
        public string cnsNumero { get; set; }
        public string cpfNumero { get; set; }
        public string nomeConjuge { get; set; }
        public string naturalidadeCidade { get; set; }
        public string naturalidadeUf { get; set; }
        public string? crfUf { get; set; }
        public string? crf { get; set; }
        public long idStatus { get; set; }
        public virtual Status status { get; set; }

        public long idSexo { get; set; }
        public virtual Sexo sexo { get; set; }

       
        public long idCorRaca { get; set; }
        public virtual CorRaca corraca { get; set; }
        public long idEstadoCivil { get; set; }
        public virtual EstadoCivil estadocivil { get; set; }

        public long idEstabelecimentoSaude { get; set; }
        public virtual EstabelecimentoSaude estabelecimentosaude { get; set; }


        public virtual ICollection<Contato> contato { get; set; } = new List<Contato>();
        public virtual ICollection<Endereco> endereco { get; set; } = new List<Endereco>();

        [JsonIgnore]
        public virtual ICollection<Usuario> usuario { get; set; } = new List<Usuario>();
    }
}
