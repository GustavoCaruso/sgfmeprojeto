namespace SGFME.Domain.Entidades;

public class Dispensacao : BaseEntity
{
    public long idPaciente { get; set; }
    public virtual Paciente paciente { get; set; }
    public long idCid { get; set; }
    public virtual Cid cid { get; set; }
    public DateTime inicioApac { get; set; }
    public DateTime fimApac { get; set; }
    public string? observacao { get; set; }
    public virtual ICollection<DispensacaoMedicamento> Medicamentos { get; set; }

    public Dispensacao Atualizar(long idPaciente, long idCid, DateTime inicioApac, string? observacao, ICollection<DispensacaoMedicamento> medicamentos)
    {
        this.idPaciente = idPaciente;
        this.idCid = idCid;
        this.inicioApac = inicioApac;
        this.observacao = observacao;
        Medicamentos = medicamentos;

        return this;
    }
}
