using Newtonsoft.Json;
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
        public bool Procedencia { get; set; }
        [StringLength(256)]
        public string ResponsavelAcao { get; set; }
        public string Descricao { get; set; }
        public DateTime DataPrevista { get; set; }
        public DateTime? DataSolucao { get; set; }
        [Required]
        public int ItemLitigioId { get; set; }

        [JsonIgnore]
        public virtual ItemLitigio ItemLitigio { get; set; }

        [NotMapped]
        public StatusPlanoAcao Status { get
            {
                if (DataSolucao.HasValue)
                    return StatusPlanoAcao.Solucionado;

                var diferenca = DataPrevista.AddHours(DataPrevista.Hour * -1).Subtract(DateTime.Today).Days;
                if (diferenca > 10)
                    return StatusPlanoAcao.NoPrazo;
                else if (diferenca < 10 && diferenca >= 0)
                    return StatusPlanoAcao.AVencer;
                else
                    return StatusPlanoAcao.Vencido;
            }
        }

    }
}
