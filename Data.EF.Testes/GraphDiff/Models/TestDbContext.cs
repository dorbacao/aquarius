using System.Data.Common;
using System.Data.Entity;

namespace Vvs.Infraestrutura.Data.EF.Testes.GraphDiff.Models
{
    public class TestDbContext : DbContext
    {
        public IDbSet<Company> Companies { get; set; }
        public IDbSet<CompanyContact> CompanyContacts { get; set; }
        public IDbSet<Project> Projects { get; set; }
        public IDbSet<Manager> Managers { get; set; }
        public IDbSet<Locker> Lockers { get; set; }
        public IDbSet<Employee> Employees { get; set; }
        public IDbSet<MultiLevelTest> MultiLevelTest { get; set; }
        public IDbSet<Hobby> Hobbies { get; set; }

        public TestDbContext(DbConnection existingConnection) : base(existingConnection, false) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().HasMany(p => p.Contacts).WithRequired().WillCascadeOnDelete(true);
            modelBuilder.Entity<CompanyContact>().HasMany(p => p.Infos).WithRequired().WillCascadeOnDelete(true);
            modelBuilder.Entity<Project>().HasMany(p => p.Stakeholders).WithMany();
            modelBuilder.Entity<Employee>().HasKey(p => p.Key);

            base.OnModelCreating(modelBuilder);
        }
    }
}
