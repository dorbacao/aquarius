using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Northwind.Tests.Domain.Entity;
using Northwind.Tests.UnitOfWork.Mapping;

namespace Northwind.Tests.UnitOfWork
{
    public class MainUnitOfWork : Vvs.Infraestrutura.Data.EF.UnitOfWork
    {
        #region ' Constructor '
        
        public MainUnitOfWork(DbConnection connection, bool contextOwnsConnection) : base(connection, contextOwnsConnection) { }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Relationships
            modelBuilder.Configurations.Add(new RegionTypeConfiguration());
            modelBuilder.Configurations.Add(new TerritoriesTypeConfiguration());
            modelBuilder.Configurations.Add(new PessoaTypeConfiguration());
            modelBuilder.Configurations.Add(new ClienteTypeConfiguration());
            modelBuilder.Configurations.Add(new FornecedorTypeConfiguration());


            base.OnModelCreating(modelBuilder);
        }

        public IQueryable<Region> Regions
        {
            get { return base.Set<Region>(); }
        }

        public IQueryable<Territories> Territories
        {
            get { return base.Set<Territories>(); }
        }

    }
}
