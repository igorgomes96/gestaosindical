using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class Relatorio
    {
        public int Id { get; set; }
        public int NegociacaoId { get; set; }

        public virtual Negociacao Negociacao { get; set; }
        public virtual ICollection<GrupoPergunta> GruposPerguntas { get; set; }
    }
}
