using Aquarius.Data.Tests.Modelo;

namespace Aquarius.Data.Tests.Data
{
    public class CidadeReadonlyRepository : Vvs.Data.Repository<Cidade>
    {
        public CidadeReadonlyRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
