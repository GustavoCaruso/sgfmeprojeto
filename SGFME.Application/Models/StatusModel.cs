using SGFME.Domain.Entidades;

namespace SGFME.Application.Models
{
    public class StatusModel
    {
        public long id { get; set; }
        public string nome { get; set; }
        public virtual ICollection<Representante> representante { get; set; } = new List<Representante>();
    }
}
