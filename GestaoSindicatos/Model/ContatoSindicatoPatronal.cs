using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class ContatoSindicatoPatronal
    {
        public int Id { get; set; }
        [Required]
        public int ContatoId { get; set; }
        [Required]
        public int SindicatoPatronalId { get; set; }

        public virtual Contato Contato { get; set; }
        public virtual SindicatoPatronal SindicatoPatronal { get; set; }
    }
}
