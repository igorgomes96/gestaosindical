using GestaoSindicatos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class ContatosService : CrudService<Contato>
    {
        private readonly Context _db;
        private readonly CrudService<ContatoEmpresa> _empresaSer;
        private readonly CrudService<ContatoSindicatoLaboral> _laboralSer;
        private readonly CrudService<ContatoSindicatoPatronal> _patronalSer;

        public ContatosService(Context db,
            CrudService<ContatoEmpresa> empresaSer,
            CrudService<ContatoSindicatoLaboral> laboralSer,
            CrudService<ContatoSindicatoPatronal> patronalSer) : base(db)
        {
            _db = db;
            _laboralSer = laboralSer;
            _patronalSer = patronalSer;
            _empresaSer = empresaSer;
        }

        public override Contato Delete(params object[] key)
        {
            _laboralSer.Delete(x => x.ContatoId == (int)key[0]);
            _patronalSer.Delete(x => x.ContatoId == (int)key[0]);
            _empresaSer.Delete(x => x.ContatoId == (int)key[0]);
            return base.Delete(key);
        }
    }
}
