using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class Reajuste
    {
        public int Id { get; set; }
        public float Salario { get; set; }
        public float Piso { get; set; }
        public float AuxCreche { get; set; }
        public float VaVr { get; set; }
        public bool VaVrFerias { get; set; }
        public float DescontoVt { get; set; }

    }
}
