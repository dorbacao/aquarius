using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Northwind.Tests.UoW;

namespace Northwind.Tests.Initializers
{
    [TestClass]
    public class AssemblyTestsInitialize
    {
        [AssemblyInitialize()]
        public static void RebuildUnitOfWork(TestContext context)
        {
            //Registra o provider do Effort
            Effort.Provider.EffortProviderConfiguration.RegisterProvider(); 

            //Set default initializer for MainBCUnitOfWork
            Database.SetInitializer<MainUnitOfWork>(new MainUnitOfWorkInitializer());
        }
    }
}
