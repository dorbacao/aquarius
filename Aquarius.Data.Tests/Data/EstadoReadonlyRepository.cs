using Aquarius.Data.Tests.Modelo;

namespace Aquarius.Data.Tests.Data
{
    public class EstadoReadonlyRepository : Vvs.Data.ReadonlyRepository<Estado>
    {
        public EstadoReadonlyRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
