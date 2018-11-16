using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public enum EntityType
    {
        Empresa,
        SindicatoLaboral,
        SindicatoPatronal
    }

    public class PesquisaResult
    {
        public EntityType EntityType { get; set; }
        public object Obj { get; set; }
    }
}
