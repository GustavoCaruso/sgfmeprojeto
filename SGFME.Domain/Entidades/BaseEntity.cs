using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public abstract class BaseEntity
    {
        public virtual long id { get; set; }
    }
}
