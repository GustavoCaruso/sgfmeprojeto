namespace SGFME.Application.DTOs
{
    public class EnderecoCreateDTO
    {
        public long idTipoEndereco { get; set; } // ID do tipo de contato
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
        public string pontoReferencia { get; set; }
    }
}
