using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public enum AplicacaoResposta
    {
        Sim,
        Nao,
        NA
    }

    public class RespostaRelatorio
    {
        public int Id { get; set; }
        [Required]
        public int Ordem { get; set; }
        [Required]
        [StringLength(1000)]
        public string Pergunta { get; set; }
        [StringLength(4000)]
        public string Resposta { get; set; }
        [Required]
        public AplicacaoResposta AplicacaoResposta { get; set; }
        [Required]
        public int GrupoPerguntaId { get; set; }
        [Required]
        public int NumColunas { get; set; } = 12;  // Qtda de colunas que a pergunta ocupa (limitado a 12)

        public virtual GrupoPergunta GrupoPergunta { get; set; }

    }
}
