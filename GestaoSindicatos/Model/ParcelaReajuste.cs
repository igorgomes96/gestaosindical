using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public enum TipoReajuste
    {
        Salario,
        Piso,
        Creche,
        VaVr,
        VaVrFerias,
        VT
    }

    public class ParcelaReajuste
    {
        public int Id { get; set; }
        [Required]
        public Mes Mes { get; set; }
        [Required]
        public float Valor { get; set; }
        [Required]
        public int ReajusteId { get; set; }
        public TipoReajuste TipoReajuste { get; set; }

        public virtual Reajuste Reajuste { get; set; }
    }
}
