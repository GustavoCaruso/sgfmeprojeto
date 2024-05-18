namespace SGFME.Application.DTOs
{
    public class PacienteCreateDTO
    {
        public string nomeCompleto { get; set; }
        public string sexo { get; set; }
        public string rg { get; set; }
        public string cpf { get; set; }
        public string cns { get; set; }
        public decimal peso { get; set; }
        public decimal altura { get; set; }
        public DateTime dataNascimento { get; set; }
        public string naturalidade { get; set; }
        public string ufNaturalidade { get; set; }
        public string corRaca { get; set; }
        public string estadoCivil { get; set; }
        public string nomeMae { get; set; }
        public List<ContatoCreateDTO> contato { get; set; } = new List<ContatoCreateDTO>();
    }

}
