namespace SGFME.Domain.Entidades
{
    public class DispensacaoMedicamento : BaseEntity
    {
        public long IdMedicamento { get; set; }
        public virtual Medicamento Medicamento { get; set; }
        public int quantidade { get; set; }
        public bool recibo { get; set; }
        public bool recebido { get; set; }
        public long idDispensacao { get; set; }
    }
}
