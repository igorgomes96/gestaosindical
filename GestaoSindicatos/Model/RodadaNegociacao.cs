using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class RodadaNegociacao
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int NegociacaoId { get; set; }
        public int Numero { get; set; }
        [Required]
        public DateTime Data { get; set; }
        public string Resumo { get; set; }
        public float? CustosViagens { get; set; }

        [JsonIgnore]
        [ForeignKey("NegociacaoId")]
        public virtual Negociacao Negociacao { get; set; }
    }
}
