using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class ReajusteConcorrente
    {
        public int Id { get; set; }
        [Required]
        public int ReajusteId { get; set; }
        [Required]
        public int NegociacaoId { get; set; }

        public virtual Negociacao Negociacao { get; set; }
        public virtual Reajuste Reajuste { get; set; }
    }
}
