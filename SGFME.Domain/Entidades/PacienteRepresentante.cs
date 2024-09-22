using System;

namespace SGFME.Domain.Entidades
{
    public class PacienteRepresentante : BaseEntity
    {
        public long idPaciente { get; set; }
        public virtual Paciente paciente { get; set; }

        public long idRepresentante { get; set; }
        public virtual Representante representante { get; set; }
    }
}
