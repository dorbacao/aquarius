using Vvs.Data.Tests.Modelo;

namespace Vvs.Data.Tests.Data
{
    public class EstadoReadonlyRepository : Vvs.Data.ReadonlyRepository<Estado>
    {
        public EstadoReadonlyRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
