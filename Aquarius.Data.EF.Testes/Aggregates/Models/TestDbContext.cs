using System.Data.Common;
using System.Data.Entity;

namespace Aquarius.Data.EF.Testes.Aggregates.Models
{
    public class TestDbContext : UnitOfWork
    {
        public IDbSet<Company> Companies { get; set; }
        public IDbSet<CompanyAddress> CompanyAddresses { get; set; }
        public IDbSet<CompanyContact> CompanyContacts { get; set; }
        public IDbSet<Project> Projects { get; set; }
        public IDbSet<Manager> Managers { get; set; }
        public IDbSet<Employee> Employees { get; set; }

        #region ' Constructor '

        public TestDbContext(DbConnection existingConnection) : base(existingConnection, false) { }
        public TestDbContext(DbConnection existingConnection, IAggregateUpdateStrategy aggregateUpdateStrategy) : base(existingConnection, false, aggregateUpdateStrategy) { }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().HasMany(p => p.Contacts).WithRequired().WillCascadeOnDelete(true);
            modelBuilder.Entity<Company>().HasOptional(c => c.Address).WithRequired().WillCascadeOnDelete(true);

            modelBuilder.Entity<CompanyAddress>().HasKey(a => a.Id);

            modelBuilder.Entity<CompanyContactBase>().HasMany(p => p.Infos).WithRequired().WillCascadeOnDelete(true);
            modelBuilder.Entity<Project>().HasMany(p => p.Stakeholders).WithMany();
            modelBuilder.Entity<Employee>().HasKey(p => p.Key);

            base.OnModelCreating(modelBuilder);
        }
    }
}
