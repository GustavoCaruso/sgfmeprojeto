﻿using SGFME.Domain.Entidades;

namespace SGFME.Application.Models
{
    public class RepresentanteModel
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
        public long idStatus { get; set; }
        public virtual Status status { get; set; }

        public long idSexo { get; set; }
        public virtual Sexo sexo { get; set; }

        public long idProfissao { get; set; }
        public virtual Profissao profissao { get; set; }
        public long idCorRaca { get; set; }
        public virtual CorRaca corraca { get; set; }
        public long idEstadoCivil { get; set; }
        public virtual EstadoCivil estadocivil { get; set; }
        public virtual ICollection<Contato> contato { get; set; } = new List<Contato>();
        public virtual ICollection<Endereco> endereco { get; set; } = new List<Endereco>();
        public virtual ICollection<PacienteRepresentanteModel> pacienterepresentante { get; set; } = new List<PacienteRepresentanteModel>();
    }
}
