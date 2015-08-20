using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Northwind.Tests.UnitOfWork;

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
