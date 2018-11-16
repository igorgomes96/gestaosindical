using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class SindicatoPatronal
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        [StringLength(14)]
        public string Cnpj { get; set; }
        [StringLength(30)]
        public string Telefone1 { get; set; }
        [StringLength(30)]
        public string Telefone2 { get; set; }
        [StringLength(150)]
        public string Gestao { get; set; }
        [StringLength(150)]
        public string Site { get; set; }

        [NotMapped]
        public Contato Presidente { get; set; }
        [NotMapped]
        public Contato Negociador { get; set; }

    }
}
