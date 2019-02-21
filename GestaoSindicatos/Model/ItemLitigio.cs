using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class ItemLitigio
    {
        public int Id { get; set; }
        [Required]
        public int LitigioId { get; set; }
        public string Assuntos { get; set; }
        public int PlanoAcaoId { get; set; }
        public bool PossuiPlano { get; set; }

        [JsonIgnore]
        public virtual Litigio Litigio { get; set; }
        public virtual PlanoAcao PlanoAcao { get; set; }
    }
}
