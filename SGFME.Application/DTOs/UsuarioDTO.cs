namespace SGFME.Application.DTOs
{
    public class UsuarioDTO
    {
        public long id { get; set; }
        public string nomeUsuario { get; set; }
        public string senha { get; set; }

        public long idStatus { get; set; }


        public long idPerfilUsuario { get; set; }


        public long idFuncionario { get; set; }

        // Nova propriedade para indicar se o usuário precisa trocar a senha
        public bool PrecisaTrocarSenha { get; set; }
    }
}
