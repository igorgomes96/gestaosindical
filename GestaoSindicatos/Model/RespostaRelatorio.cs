using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public enum AplicacaoReposta
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
        public AplicacaoReposta AplicacaoResposta { get; set; }
        [Required]
        public int GrupoPerguntaId { get; set; }

        public virtual GrupoPergunta GrupoPergunta { get; set; }

    }
}
