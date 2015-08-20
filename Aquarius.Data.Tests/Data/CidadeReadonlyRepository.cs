using Vvs.Data.Tests.Modelo;

namespace Vvs.Data.Tests.Data
{
    public class CidadeReadonlyRepository : Vvs.Data.Repository<Cidade>
    {
        public CidadeReadonlyRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
