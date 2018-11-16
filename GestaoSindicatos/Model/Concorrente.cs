using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class Concorrente
    {
        public int Id { get; set; }
        [StringLength(200)]
        public string Nome { get; set; }
        [Required]
        public int NegociacaoId { get; set; }
        [Required]
        public int ReajusteId { get; set; }

        public virtual Negociacao Negociacao { get; set; }
        public virtual Reajuste Reajuste { get; set; }
    }
}
