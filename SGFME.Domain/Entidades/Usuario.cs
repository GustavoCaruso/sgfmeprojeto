using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Usuario : BaseEntity
    {
        public string nomeUsuario { get; set; }
        public string senha { get; set; }
        public long idStatus { get; set; }
        public virtual Status status { get; set; }
        public long idPerfilUsuario { get; set; }
        public virtual PerfilUsuario perfilusuario { get; set; }
        public long idFuncionario { get; set; }
        public virtual Funcionario funcionario { get; set; }
        // Nova propriedade para indicar se a senha precisa ser trocada
        public bool precisaTrocarSenha { get; set; } = true; // Padrão é true para novos usuários
    }
}
