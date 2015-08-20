using Aquarius.Data.Tests.Modelo;

namespace Aquarius.Data.Tests.Data
{
    public class PaisReadonlyRepository : Vvs.Data.ReadonlyRepository<Pais>
    {
        public PaisReadonlyRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
