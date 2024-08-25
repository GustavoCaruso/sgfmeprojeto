﻿namespace SGFME.Application.Models
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
    }
}
