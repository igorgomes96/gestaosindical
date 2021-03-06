﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestaoSindicatos.Model
{
    public class Relatorio
    {
        public int Id { get; set; }
        public int NegociacaoId { get; set; }
        [StringLength(100)]
        public string Titulo { get; set; }

        public virtual Negociacao Negociacao { get; set; }
        public virtual ICollection<GrupoPergunta> GruposPerguntas { get; set; }
    }
}
