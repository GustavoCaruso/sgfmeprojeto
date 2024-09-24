using SGFME.Domain.Entidades;

namespace SGFME.Application.DTOs;

public class CriarDispensacaoMedicamentoDTO
{
    public long idMedicamento { get; set; }
    public int quantidade { get; set; }
    public bool recibo { get; set; }
    public bool recebido { get; set; }
}
