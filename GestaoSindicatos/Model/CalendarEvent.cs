using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class Color
    {
        public string Primary { get; set; }
        public string Secondary { get; set; }

    }

    public class CalendarEvent
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Title { get; set; }
        public Color Color { get; set; }
        public bool AllDay { get; set; }
    }
}
