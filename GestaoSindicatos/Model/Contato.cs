using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public enum TipoContato
    {
        Presidente,
        Negociador,
        Contato,
        Outro
    }
    public class Contato
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Nome { get; set; }
        [Required]
        public TipoContato TipoContato { get; set; }
        [StringLength(30)]
        public string Telefone1 { get; set; }
        [StringLength(30)]
        public string Telefone2 { get; set; }
        [StringLength(150)]
        public string Email { get; set; }


    }
}
