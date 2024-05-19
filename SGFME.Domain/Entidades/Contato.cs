using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Contato : BaseEntity
    {
        public string valor { get; set; }

       
        //Relacionamento com a entidade Paciente-One to Many
        public long idPaciente { get; set; } // Chave estrangeira
        public virtual Paciente paciente { get; set; } // Propriedade de navegação


        //Relacionamento com a entidade TipoEndereco-One to one
        public long idTipoContato { get; set; } // Chave estrangeira
        public virtual TipoContato tipocontato { get; set; } // Propriedade de navegação
    }
}
