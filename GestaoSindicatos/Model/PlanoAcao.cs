using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public enum StatusPlanoAcao
    {
        NoPrazo,
        Vencido,
        AVencer,  // 10 dias antes da data
        Solucionado
    }

    public class PlanoAcao
    {
        public int Id { get; set; }
        [Required]
        public DateTime Data { get; set; }
        [Required]
        [MaxLength(2), MinLength(2)]
        public string Estado { get; set; }
        [Required]
        public Referente Referente { get; set; }
        public int? LaboralId { get; set; }
        public int? PatronalId { get; set; }
        public string Reclamacoes { get; set; }
        [StringLength(256)]
        public string Reclamante { get; set; }
        [Required]
        public bool Procedencia { get; set; }
        [StringLength(256)]
        public string ResponsavelAcao { get; set; }
        public DateTime? DataSolucao { get; set; }

        public virtual SindicatoLaboral Laboral { get; set; }
        public virtual SindicatoPatronal Patronal { get; set; }

        [NotMapped]
        public StatusPlanoAcao Status { get
            {
                if (DataSolucao.HasValue)
                    return StatusPlanoAcao.Solucionado;

                var hoje = DateTime.Today;
                var diferenca = Data.Subtract(hoje).Days;
                if (diferenca > 10)
                    return StatusPlanoAcao.NoPrazo;
                else if (diferenca < 10 && diferenca > 0)
                    return StatusPlanoAcao.AVencer;
                else
                    return StatusPlanoAcao.Vencido;
            }
        }

    }
}
