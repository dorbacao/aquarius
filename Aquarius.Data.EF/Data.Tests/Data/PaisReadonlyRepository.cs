using System.Collections.Generic;
using Vvs.Data.Tests.Modelo;

namespace Vvs.Data.Tests.Data
{
    public class PaisReadonlyRepository : Vvs.Data.ReadonlyRepository<Pais>
    {
        public PaisReadonlyRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
