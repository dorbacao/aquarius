using Aquarius.Data.Tests.Modelo;
using Aquarius.Seedwork.Repositorios;
using Aquarius.Seedwork.UnitOfWork;

namespace Aquarius.Data.Tests.Data
{
    public class EstadoReadonlyRepository : Repository<Estado>
    {
        public EstadoReadonlyRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
