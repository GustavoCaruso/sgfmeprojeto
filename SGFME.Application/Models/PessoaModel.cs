namespace SGFME.Application.Models
{
    public class PessoaModel
    {
        public long id { get; set; }
        public string nomeCompleto { get; set; }
        public DateTime dataNascimento { get; set; }

        public string nomeMae { get; set; }

        // Calculando a idade com base na data de nascimento
        public int idade { get; set; }
    }
}
