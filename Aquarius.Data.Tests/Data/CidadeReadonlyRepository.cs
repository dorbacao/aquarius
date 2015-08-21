using Aquarius.Data.Tests.Modelo;
using Aquarius.Seedwork.Repositorios;
using Aquarius.Seedwork.UnitOfWork;

namespace Aquarius.Data.Tests.Data
{
    public class CidadeReadonlyRepository : Repository<Cidade>
    {
        public CidadeReadonlyRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
