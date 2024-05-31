﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Entidades
{
    public class Status : BaseEntity
    {
        public string nome { get; set; }
        public virtual ICollection<Representante> representante { get; set; } = new List<Representante>();
    }
}
