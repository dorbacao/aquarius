using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using Vvs.Infraestrutura.Data.SqlClient.Tests.Modelo;
using Vvs.Infraestrutura.Data.SqlClient.Tests.UnitOfWork.Mapping;

namespace Vvs.Infraestrutura.Data.SqlClient.Tests.UnitOfWork
{
    public class MainUnitOfWork : EF.UnitOfWork, IDisposable
    {        
        public MainUnitOfWork(string nameOrConnectionString) : base(nameOrConnectionString) { } 
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            base.Configuration.ProxyCreationEnabled = false;

            Database.SetInitializer(new DropCreateDatabaseAlways<MainUnitOfWork>());            

            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new EntidadeTesteConfiguration());
            modelBuilder.Configurations.Add(new EntityTypeConfiguration<EntidadeTesteDataAnnotation>());
        }
    }

}
