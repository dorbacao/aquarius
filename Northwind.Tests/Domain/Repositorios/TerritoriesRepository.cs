using Aquarius.Seedwork.Repositorios;
using Aquarius.Seedwork.UnitOfWork;
using Northwind.Tests.Domain.Entity;

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
