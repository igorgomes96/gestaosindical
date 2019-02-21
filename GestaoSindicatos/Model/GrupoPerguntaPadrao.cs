using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class GrupoPerguntaPadrao
    {
        public int Id { get; set; }
        [Required]
        public int Ordem { get; set; }
        [Required]
        [StringLength(400)]
        public string Texto { get; set; }

        public virtual ICollection<PerguntaPadrao> Perguntas { get; set; }
    }
}
