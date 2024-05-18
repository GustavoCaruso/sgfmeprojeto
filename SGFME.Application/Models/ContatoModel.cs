using SGFME.Domain.Entidades;
using System.Text.Json.Serialization;

namespace SGFME.Application.Models
{
    public class ContatoModel
    {
        public long id { get; set; }
        public string tipo { get; set; }
        public string valor { get; set; }

        public long idPaciente { get; set; } // Chave estrangeira
        
        public virtual Paciente paciente { get; set; } // Propriedade de navegação



        public long idTipoContato { get; set; } // Chave estrangeira
        public virtual TipoContato tipocontato { get; set; } // Propriedade de navegação
    }
}
