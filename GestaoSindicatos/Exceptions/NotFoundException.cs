using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("Registro não encontrado!") { }
        public NotFoundException(string message): base(message) { }
    }
}
