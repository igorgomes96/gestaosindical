using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class Empresa
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(14), MinLength(14)]
        public string Cnpj { get; set; }
        [Required]
        [StringLength(200)]
        public string Nome { get; set; }
        [Required]
        public int EnderecoId { get; set; }
        public int QtdaTrabalhadores { get; set; }
        public double MassaSalarial { get; set; }
        public int? SindicatoLaboralId { get; set; }
        public int? SindicatoPatronalId { get; set; }
        public Mes Database { get; set; }

        public virtual SindicatoLaboral SindicatoLaboral { get; set; }
        public virtual SindicatoPatronal SindicatoPatronal { get; set; }
        public virtual Endereco Endereco { get; set; }
        
    }
}
