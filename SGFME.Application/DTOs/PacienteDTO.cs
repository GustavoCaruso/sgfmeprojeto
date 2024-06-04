using SGFME.Domain.Entidades;

namespace SGFME.Application.DTOs
{
    public class PacienteDTO
    {
        public long id { get; set; }
        public string nomeCompleto { get; set; }
        public decimal peso { get; set; }
        public decimal altura { get; set; }
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




        public long idStatus { get; set; }
        

        public long idSexo { get; set; }
        

        public long idProfissao { get; set; }
        
        public long idCorRaca { get; set; }
        
        public long idEstadoCivil { get; set; }
        


        public List<ContatoCreateDTO> contato { get; set; } = new List<ContatoCreateDTO>();
        public List<EnderecoCreateDTO> endereco { get; set; } = new List<EnderecoCreateDTO>();
    }

}
