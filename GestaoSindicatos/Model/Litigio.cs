using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public enum ProcedimentoLitigio
    {
        MesaRedonda = 1,
        Audiencia = 2,
        ReclamacaoSindicato = 3
    }

    public enum Referente
    {
        MTE,
        MPT,
        Laboral,
        Patronal
    }

    public class Litigio
    {
        public int Id { get; set; }
        [Required]
        public int EmpresaId { get; set; }
        public int? LaboralId { get; set; }
        public int? PatronalId { get; set; }
        [Required]
        public Referente Referente { get; set; }
        [Required]
        public ProcedimentoLitigio Procedimento { get; set; }
        [Required]
        public DateTime Data { get; set; }
        public string Assuntos { get; set; }
        public string ResumoAssuntos { get; set; }
        public string Participantes { get; set; }

        public virtual Empresa Empresa { get; set; }
        public virtual SindicatoLaboral Laboral { get; set; }
        public virtual SindicatoPatronal Patronal { get; set; }


    }
}
