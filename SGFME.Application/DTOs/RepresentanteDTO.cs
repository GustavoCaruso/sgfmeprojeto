namespace SGFME.Application.DTOs
{
    public class RepresentanteDTO
    {
        public long id { get; set; }
        public string nomeCompleto { get; set; }
        public DateTime dataNascimento { get; set; }
        public DateTime dataCadastro { get; set; }
        public string rgNumero { get; set; }
        public DateTime rgDataEmissao { get; set; }
        public string rgOrgaoExpedidor { get; set; }
        public string rgUfEmissao { get; set; }
        public string cnsNumero { get; set; }
        public string cpfNumero { get; set; }
        public long idStatus { get; set; } // Novo campo para StatusId
        public List<ContatoCreateDTO> contato { get; set; } = new List<ContatoCreateDTO>();
        public List<EnderecoCreateDTO> endereco { get; set; } = new List<EnderecoCreateDTO>();

    }
}
