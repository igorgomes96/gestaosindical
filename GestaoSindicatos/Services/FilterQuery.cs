using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class FilterQuery
    {
        private static bool Compare(Tuple<object, object> values)
        {
            if (values == null) return true;
            if (values.Item2 == null) return true;
            if (values.Item1 is string)
            {
                return (values.Item1 as string).ToLower().Contains(values.Item2.ToString().ToLower().Trim());
            } else
            {
                return values.Item1 != null && values.Item1.ToString() == values.Item2.ToString();
            }
        }

        public static bool And(params Tuple<object, object>[] values)
        {
            return values.All(v => Compare(v));
        }

        public static bool Or(params Tuple<object, object>[] values)
        {
            return values.Any(v => Compare(v));
        }
    }
}
