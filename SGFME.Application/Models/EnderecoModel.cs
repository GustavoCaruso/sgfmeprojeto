namespace SGFME.Application.Models
{
    public class EnderecoModel
    {
        public long id { get; set; }
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
