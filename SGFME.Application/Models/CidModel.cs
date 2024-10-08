﻿using SGFME.Domain.Entidades;

namespace SGFME.Application.Models
{
    public class CidModel
    {
        public long id { get; set; }
        public string codigo { get; set; }
        public string descricao { get; set; }
        public long idStatus { get; set; }
        public virtual Status status { get; set; }

        public long idVersaoCid { get; set; }
        public virtual VersaoCid versaocid { get; set; }
    }
}
