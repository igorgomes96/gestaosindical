using GestaoSindicatos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class ReajustesService: CrudService<Reajuste>
    {
        private readonly Context _db;

        public ReajustesService(Context db): base(db)
        {
            _db = db;
        }

        public override Reajuste Delete(params object[] key)
        {
            _db.ParcelasReajustes.Where(x => x.ReajusteId == (int)key[0])
                .ToList().ForEach(x => _db.ParcelasReajustes.Remove(x));

            _db.SaveChanges();

            return base.Delete(key);
        }

        public override void Delete(Expression<Func<Reajuste, bool>> query)
        {
            Query(query).ToList().ForEach(x => Delete(x.Id));
        }
    }
}
