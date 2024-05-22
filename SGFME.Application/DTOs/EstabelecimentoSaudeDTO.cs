namespace SGFME.Application.DTOs
{
    public class EstabelecimentoSaudeDTO
    {
        public long id { get; set; }
        public String nomeFantasia { get; set; }
        public String razaoSocial { get; set; }
        public String cnes { get; set; }

        public List<ContatoCreateDTO> contato { get; set; } = new List<ContatoCreateDTO>();
    }
}
