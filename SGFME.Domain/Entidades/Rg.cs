using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Rg : BaseEntity
    {
        public string numero { get; set; }
        public DateTime dataEmissao { get; set; }
        public string orgaoExpedidor { get; set; }
        public string ufEmissao { get; set; }


       
    }
}
