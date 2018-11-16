using GestaoSindicatos.Auth;
using GestaoSindicatos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class LitigiosService : CrudService<Litigio>
    {
        private readonly Context _db;
        public LitigiosService(Context db) : base(db)
        {
            _db = db;
        }

        private Expression<Func<Litigio, bool>> QueryUser(ClaimsPrincipal claims)
        {
            return (Litigio n) => _db.EmpresasUsuarios.Any(u => u.UserName == claims.Identity.Name && u.EmpresaId == n.EmpresaId);
        }

        public IQueryable<Litigio> Query(Expression<Func<Litigio, bool>>  query, ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query(query).Where(QueryUser(claims));

            return base.Query(query);
        }

        public IQueryable<Litigio> Query(ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query().Where(QueryUser(claims));

            return base.Query();
        }

        public override Litigio Find(params object[] key)
        {
            Litigio litigio = base.Find(key);
            _db.Entry(litigio).Reference(l => l.Empresa).Load();
            _db.Entry(litigio).Reference(l => l.Laboral).Load();
            _db.Entry(litigio).Reference(l => l.Patronal).Load();
            return litigio;
        }
    }
}
