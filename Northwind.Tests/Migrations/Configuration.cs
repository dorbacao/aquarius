using System.Data.Entity.Migrations;
using Northwind.Tests.UnitOfWork;

namespace Northwind.Tests.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<MainUnitOfWork>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(MainUnitOfWork context)
        {

        }

    }
}
