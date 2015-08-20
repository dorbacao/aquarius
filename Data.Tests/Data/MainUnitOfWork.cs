using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using NUnit.Framework.Constraints;
using Vvs.Data.Tests.Modelo;

namespace Vvs.Data.Tests.Data
{
    public class MainUnitOfWork : Vvs.Data.EF.UnitOfWork
    {

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new CidadeConfiguration());
            modelBuilder.Configurations.Add(new EstadoConfiguration());
            modelBuilder.Configurations.Add(new EntityTypeConfiguration<Modelo.Pais>());

        }


        public class CidadeConfiguration : EntityTypeConfiguration<Modelo.Cidade>
        {

            public CidadeConfiguration()
            {
                HasKey(c => c.Id);
                Property(c => c.NomeCidade).IsRequired();
                HasRequired(c => c.Estado).WithMany(e => e.Cidades);

            }

        }


        public class EstadoConfiguration : EntityTypeConfiguration<Modelo.Estado>
        {

            public EstadoConfiguration()
            {
                HasKey(c => c.Id);
                Property(c => c.NomeEstado).IsRequired();
                Property(c => c.UF).IsRequired();
                HasOptional(e => e.Pais);
            }

        }


    }
}
