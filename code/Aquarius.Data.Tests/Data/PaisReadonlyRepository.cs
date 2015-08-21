using Aquarius.Data.Tests.Modelo;
using Aquarius.Seedwork.Repositorios;
using Aquarius.Seedwork.UnitOfWork;

namespace Aquarius.Data.Tests.Data
{
    public class PaisReadonlyRepository : Repository<Pais>
    {
        public PaisReadonlyRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
