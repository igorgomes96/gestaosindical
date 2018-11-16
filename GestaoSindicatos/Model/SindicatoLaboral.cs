using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public enum CCT_ACT
    {
        ACT,
        CCT
    }

    public enum Mes
    {
        Janeiro,
        Fevereiro,
        Marco,
        Abril,
        Maio,
        Junho,
        Julho,
        Agosto,
        Setembro,
        Outubro,
        Novembro,
        Dezembro
    }

    public class SindicatoLaboral
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        [MaxLength(14)]
        public string Cnpj { get; set; }
        [StringLength(30)]
        public string Telefone1 { get; set; }
        [StringLength(30)]
        public string Telefone2 { get; set; }
        [StringLength(150)]
        public string Gestao { get; set; }
        [StringLength(150)]
        public string Site { get; set; }
        [StringLength(150)]
        public string Federacao { get; set; }
        [Required]
        public CCT_ACT Cct_act { get; set; }
        [Required]
        public Mes Database { get; set; }

    }
}
