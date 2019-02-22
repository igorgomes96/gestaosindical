using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class PerguntaPadrao
    {
        public int Id { get; set; }
        [Required]
        public int Ordem { get; set; }
        [Required]
        [StringLength(1000)]
        public string Texto { get; set; }
        public int? GrupoPerguntaId { get; set; }
        [Required]
        public int NumColunas { get; set; } = 12;  // Qtda de colunas que a pergunta ocupa (limitado a 12)

        public virtual GrupoPerguntaPadrao GrupoPergunta { get; set; }

    }
}
