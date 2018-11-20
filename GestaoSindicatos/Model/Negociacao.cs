using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{

    public enum StatusNegociacao
    {
        NaoIniciada,
        EmNegociacao,
        Fechada,
        Dissidio

    }
    public class Negociacao
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Ano { get; set; }
        public int? EmpresaId { get; set; }
        public int? SindicatoLaboralId { get; set; }
        public int? SindicatoPatronalId { get; set; }
        [Required]
        public int QtdaTrabalhadores { get; set; }
        [Required]
        public double MassaSalarial { get; set; }
        public int? QtdaRodadas { get; set; }
        public float? TaxaLaboral { get; set; }
        public float? TaxaPatronal { get; set; }
        public int? OrcadoId { get; set; }
        public int? NegociadoId { get; set; }
        public float? Plr1Sem { get; set; }
        public float? Plr2Sem { get; set; }
        [Required]
        public StatusNegociacao StatusACT_CCT { get; set; }
        [Required]
        public StatusNegociacao StatusPLR { get; set; }


        public virtual Empresa Empresa { get; set; }
        public virtual SindicatoLaboral SindicatoLaboral { get; set; }
        public virtual SindicatoPatronal SindicatoPatronal { get; set; }
        [ForeignKey("OrcadoId")]
        public virtual Reajuste Orcado { get; set; }
        [ForeignKey("NegociadoId")]
        public virtual Reajuste Negociado { get; set; }

        public virtual ICollection<Concorrente> Concorrentes { get; set; }
        [JsonIgnore]
        public virtual ICollection<RodadaNegociacao> RodadasNegociacoes { get; set; }

        [NotMapped]
        public float CustosViagens
        {
            get
            {
                if (RodadasNegociacoes == null || RodadasNegociacoes.Count <= 0)
                    return 0F;

                return RodadasNegociacoes.Sum(r => r.CustosViagens ?? 0);
            }
        }
    }
}
