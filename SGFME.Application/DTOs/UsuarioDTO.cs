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
    }
}
