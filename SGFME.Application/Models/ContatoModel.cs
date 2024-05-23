using SGFME.Domain.Entidades;
using System.Text.Json.Serialization;

namespace SGFME.Application.Models
{
    public class ContatoModel
    {
        public long id { get; set; }
        public string tipo { get; set; }
        public string valor { get; set; }

        public long? idPaciente { get; set; } // Chave estrangeira
        public virtual Paciente paciente { get; set; } // Propriedade de navegação
        public long? idMedico { get; set; } // Chave estrangeira
        public virtual Medico medico { get; set; } // Propriedade de navegação

        public string discriminator { get; set; } // Propriedade discriminadora

        public long? idTipoContato { get; set; } // Chave estrangeira
        public virtual TipoContato tipocontato { get; set; } // Propriedade de navegação

        public long? idEstabelecimentoSaude { get; set; } // Chave estrangeira
        public virtual EstabelecimentoSaude estabelecimentosaude { get; set; } // Propriedade de navegação
    }
}
