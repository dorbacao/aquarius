using System.Data.Entity;
using Northwind.Tests.UoW;

namespace Northwind.Tests.Initializers
{
    public class MainUnitOfWorkInitializer : DropCreateDatabaseAlways<MainUnitOfWork>
    {
        protected override void Seed(MainUnitOfWork unitOfWork)
        {

        }
    }
}
