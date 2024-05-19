namespace SGFME.Application.DTOs
{
    public class PacienteCreateDTO
    {
        public string nomeCompleto { get; set; }

        public decimal peso { get; set; }
        public decimal altura { get; set; }

        public DateTime dataNascimento { get; set; }
        public int idade { get; set; }

        public string nomeMae { get; set; }

        public string cnsNumero { get; set; } // Novo campo para número do CNS
        public string rgNumero { get; set; }
        public DateTime rgDataEmissao { get; set; }
        public string rgOrgaoExpedidor { get; set; }
        public string rgUfEmissao { get; set; }


        public List<ContatoCreateDTO> contato { get; set; } = new List<ContatoCreateDTO>();
        public List<EnderecoCreateDTO> endereco { get; set; } = new List<EnderecoCreateDTO>();
    }

}
