using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class GrupoPergunta
    {
        public int Id { get; set; }
        [Required]
        public int RelatorioId { get; set; }
        [Required]
        public int Ordem { get; set; }
        [Required]
        [StringLength(400)]
        public string Texto { get; set; }

        public virtual Relatorio Relatorio { get; set; }
        public virtual ICollection<RespostaRelatorio> Respostas { get; set; }
    }
}
