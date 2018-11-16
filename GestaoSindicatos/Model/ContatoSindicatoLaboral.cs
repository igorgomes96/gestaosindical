using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class ContatoSindicatoLaboral
    {
        public int Id { get; set; }
        [Required]
        public int ContatoId { get; set; }
        [Required]
        public int SindicatoLaboralId { get; set; }

        public virtual Contato Contato { get; set; }
        public virtual SindicatoLaboral SindicatoLaboral { get; set; }

    }
}
