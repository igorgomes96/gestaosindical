using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model.Dashboard
{
    public class ChartDataset
    {
        public string Label { get; set; }
        public ICollection<ChartData> Data { get; set; }
    }
}
