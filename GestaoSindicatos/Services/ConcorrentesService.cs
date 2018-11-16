using GestaoSindicatos.Exceptions;
using GestaoSindicatos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class ConcorrentesService : CrudService<Concorrente>
    {
        private readonly Context _db;
        private readonly CrudService<Reajuste> _reajustesService;
        public ConcorrentesService(Context db, CrudService<Reajuste> reajustesService) : base(db)
        {
            _db = db;
            _reajustesService = reajustesService;
        }

        public override Concorrente Delete(params object[] key)
        {
            Concorrente concorrente = base.Delete(key);
            if (concorrente == null)
                throw new NotFoundException();

            _reajustesService.Delete(concorrente.ReajusteId);

            return concorrente;
        }
    }
}
