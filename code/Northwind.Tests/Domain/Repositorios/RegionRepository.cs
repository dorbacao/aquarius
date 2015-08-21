using Aquarius.Seedwork.Repositorios;
using Aquarius.Seedwork.UnitOfWork;
using Northwind.Tests.Domain.Entity;

namespace Northwind.Tests.Domain.Repositorios
{
    public class RegionRepository : Repository<Region>
    {
          /// <summary>
        /// Cria uma Nova Instância
        /// </summary>
        /// <param name="unitOfWork">Associado ao Unit Of Work</param>
        public RegionRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
