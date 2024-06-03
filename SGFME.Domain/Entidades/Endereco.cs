using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Endereco : BaseEntity
    {
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
        public string pontoReferencia { get; set; }
        public string discriminator { get; set; } // Propriedade discriminadora

        public long? idTipoEndereco { get; set; } // Chave estrangeira
        public virtual TipoEndereco tipoendereco { get; set; } // Propriedade de navegação

        public long? idPaciente { get; set; } // Chave estrangeira
        public virtual Paciente paciente { get; set; } // Propriedade de navegação

        public long? idRepresentante { get; set; } // Chave estrangeira
        public virtual Representante representante { get; set; } // Propriedade de navegação

        public long? idMedico { get; set; } // Chave estrangeira
        public virtual Medico medico { get; set; } // Propriedade de navegação
    }
}
