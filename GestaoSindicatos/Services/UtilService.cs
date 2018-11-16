using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public static class UtilService
    {
        public static ICollection<string> ListProperty<T>(IEnumerable<T> collection, Func<T, string> property, string q = null, int takeCount = 10)
        {
            if (q == null)
                return collection.Select(property).Take(takeCount).ToList();

            return collection.Where(x => property(x).Contains(q, StringComparison.CurrentCultureIgnoreCase))
                .Select(property).Take(takeCount).ToList();
        }


    }
}
