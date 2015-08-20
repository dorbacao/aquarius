using Northwind.Tests.Domain.Entity;
using Vvs.Domain.Seedwork;
using Vvs.Domain.Seedwork.Repositorios;
using Vvs.Domain.Seedwork.UnitOfWork;

namespace Northwind.Tests.Domain.Repositorios
{
    public class TerritoriesRepository : Repository<Territories>
    {
          /// <summary>
        /// Cria uma Nova Instância
        /// </summary>
        /// <param name="unitOfWork">Associado ao Unit Of Work</param>
        public TerritoriesRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
