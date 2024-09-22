namespace SGFME.Application.Models
{
    public class PacienteRepresentanteModel
    {
        public long id { get; set; } // Herdado de BaseEntity, se necessário

        public long idPaciente { get; set; }
        public PacienteModel paciente { get; set; } // Referência ao modelo de Paciente

        public long idRepresentante { get; set; }
        public RepresentanteModel representante { get; set; } // Referência ao modelo de Representante

        
    }
}
