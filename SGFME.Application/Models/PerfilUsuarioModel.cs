using SGFME.Domain.Entidades;
using System.Text.Json.Serialization;

namespace SGFME.Application.Models
{
    public class PerfilUsuarioModel
    {
        public long id { get; set; }
        public string nome { get; set; }
        [JsonIgnore]
        public virtual ICollection<Usuario> usuario { get; set; } = new List<Usuario>();
    }
}
