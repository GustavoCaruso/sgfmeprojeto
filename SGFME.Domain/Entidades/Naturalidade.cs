using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Naturalidade : BaseEntity
    {
        public string cidade { get; set; }
        public string uf { get; set; }
    }
}
