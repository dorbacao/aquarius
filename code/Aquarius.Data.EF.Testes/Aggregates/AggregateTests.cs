using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;
using Aquarius.Data.EF.Testes.Aggregates.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aquarius.Data.EF.Testes.Aggregates
{
    /// <summary>
    /// Tests
    /// </summary>
    [TestClass]
    public class AggregateTests
    {
        #region Class construction & initialization

        private TransactionScope _transactionScope;
        private static DbConnection _connection;

        public AggregateTests()
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
            Database.SetInitializer<TestDbContext>(new DropCreateDatabaseAlways<TestDbContext>());
        }

        [ClassInitialize]
        public static void SetupTheDatabase(TestContext testContext)
        {
            _connection = Effort.DbConnectionFactory.CreateTransient();

            using (var uow = new TestDbContext(_connection))
            {
                var company1 = uow.Companies.Add(new Company
                {
                    Name = "Company 1",
                    Address = new CompanyAddress() { Id = 1, Street = "Campinho" },
                    Contacts = new List<CompanyContactBase>
                    {
                        new CompanyContact 
                        { 
                            FirstName = "Bob",
                            LastName = "Brown",
                            Infos = new List<ContactInfo>
                            {
                                new ContactInfo
                                {
                                    Description = "Home",
                                    Email = "test@test.com",
                                    PhoneNumber = "0255525255"
                                }
                            }
                        }
                    },
                });

                var company2 = uow.Companies.Add(new Company
                {
                    Name = "Company 2",
                    Contacts = new List<CompanyContactBase>
                    {
                        new CompanyContact 
                        { 
                            FirstName = "Tim",
                            LastName = "Jones",
                            Infos = new List<ContactInfo>
                            {
                                new ContactInfo
                                {
                                    Description = "Work",
                                    Email = "test@test.com",
                                    PhoneNumber = "456456456456"
                                }
                            }
                        }
                    }
                });

                var project1 = uow.Projects.Add(new Project
                {
                    Name = "Major Project 1",
                    Deadline = DateTime.Now,
                    Stakeholders = new List<Company> { company2 }
                });

                var project2 = uow.Projects.Add(new Project
                {
                    Name = "Major Project 2",
                    Deadline = DateTime.Now,
                    Stakeholders = new List<Company> { company1 }
                });

                var manager1 = uow.Managers.Add(new Manager
                {
                    PartKey = "manager1",
                    PartKey2 = 1,
                    FirstName = "Trent"
                });
                var manager2 = uow.Managers.Add(new Manager
                {
                    PartKey = "manager2",
                    PartKey2 = 2,
                    FirstName = "Timothy"
                });

                var employee = new Employee
                {
                    Manager = manager1,
                    Key = "Asdf",
                    FirstName = "Test employee",
                };

                uow.Employees.Add(employee);

                project2.LeadCoordinator = manager2;

                uow.Commit();
            }
        }

        #endregion

        #region Test Initialize and Cleanup

        [TestInitialize]
        public virtual void CreateTransactionOnTestInitialize()
        {
            _transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { Timeout = new TimeSpan(0, 10, 0) });
        }

        [TestCleanup]
        public virtual void Attached_DisposeTransactionOnTestCleanup()
        {
            Transaction.Current.Rollback();
            _transactionScope.Dispose();
        }

        #endregion


        #region ' Discover Entity Set '

        [TestMethod]
        public void DiscoverEntitySet()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var c = new Company();
                var set = ((IObjectContextAdapter) uow).ObjectContext.TryGetEntitySet(c);
                Assert.IsNotNull(set);
            }
        }

        [TestMethod]
        public void DiscoverEntitySetInheritance()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var c = new CompanyContact();
                var set = ((IObjectContextAdapter)uow).ObjectContext.TryGetEntitySet(c);
                Assert.IsNotNull(set);
            }
        }

        [TestMethod]
        public void DiscoverEntitySetInheritanceProxy()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var contact = uow.Companies.Single(p => p.Id == 2).Contacts.FirstOrDefault();
                var set = ((IObjectContextAdapter)uow).ObjectContext.TryGetEntitySet(contact);
                Assert.IsNotNull(set);
            }
        }

        #endregion

        #region ' Basic Record Update (Without relationships) '

        [TestMethod]
        public void EntityUpdate()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var company1 = uow.Companies.Single(p => p.Id == 2);
                company1.Name = "Company #1"; // Change from Company 1 to Company #1

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection))
            {
                Assert.IsTrue(uow.Companies.Single(p => p.Id == 2).Name == "Company #1");
            }
        }

        [TestMethod]
        public void DoesNotUpdateEntityIfNoChangesHaveBeenMade()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var company1 = uow.Companies.Single(p => p.Id == 2);
                
                Assert.IsTrue(uow.ChangeTracker.Entries().All(p => p.State == System.Data.Entity.EntityState.Unchanged));
            }
        }

        [TestMethod]
        public void MarksAssociatedRelationAsChangedEvenIfEntitiesAreUnchanged()
        {
            Manager manager1;

            using (var uow = new TestDbContext(_connection))
            {
                var project1 = uow.Projects.Include(m => m.LeadCoordinator).Single(p => p.Id == 1);
                manager1 = uow.Managers.First();

                project1.LeadCoordinator = manager1;

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection)) {
                Assert.AreEqual(manager1.PartKey, uow.Projects.Include(m => m.LeadCoordinator).Single(p => p.Id == 1).LeadCoordinator.PartKey);
            }
        }

        #endregion

        #region ' Associated Entity '

        [TestMethod]
        public void UpdateAssociatedEntityWherePreviousValueWasNull()
        {
            Manager coord;
            using (var uow = new TestDbContext(_connection))
            {
                var project = uow.Projects
                    .Include(p => p.LeadCoordinator)
                    .Single(p => p.Id == 1);

                coord = uow.Managers.Single(p => p.PartKey == "manager1" && p.PartKey2 == 1);

                project.LeadCoordinator = coord;

                uow.Commit();
            }


            using (var uow = new TestDbContext(_connection))
            {
                Assert.IsTrue(uow.Projects
                    .Include(p => p.LeadCoordinator)
                    .Single(p => p.Id == 1)
                    .LeadCoordinator.PartKey == coord.PartKey);
            }
        }

        [TestMethod]
        public void UpdateAssociatedEntityWhereNewValueIsNull()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var project = uow.Projects
                    .Include(p => p.LeadCoordinator)
                    .Single(p => p.Id == 2);

                Assert.IsNotNull(project.LeadCoordinator);

                project.LeadCoordinator = null;

                uow.Commit();
            }


            using (var uow = new TestDbContext(_connection))
            {
                Assert.IsTrue(uow.Projects
                    .Include(p => p.LeadCoordinator)
                    .Single(p => p.Id == 2)
                    .LeadCoordinator == null);
            }
        }

        [TestMethod]
        public void UpdateAssociatedEntityWherePreviousValueIsNewValue()
        {
            Manager coord;
            using (var uow = new TestDbContext(_connection))
            {
                Project project = uow.Projects
                    .Include(p => p.LeadCoordinator)
                    .Single(p => p.Id == 2);

                coord = uow.Managers
                    .Single(p => p.PartKey == "manager2" && p.PartKey2 == 2);

                project.LeadCoordinator = coord;

                uow.Commit();
            }
            
            using (var uow = new TestDbContext(_connection))
            {
                Assert.IsTrue(uow.Projects
                    .Include(p => p.LeadCoordinator)
                    .Single(p => p.Id == 2)
                    .LeadCoordinator.PartKey == coord.PartKey);
            }
        }

        [TestMethod]
        public void UpdateAssociatedEntityWherePreviousValueIsNotNewValue()
        {
            Manager coord;
            using (var uow = new TestDbContext(_connection))
            {
                var project = uow.Projects
                    .Include(p => p.LeadCoordinator)
                    .Single(p => p.Id == 2);

                coord = uow.Managers
                    .Single(p => p.PartKey == "manager1" && p.PartKey2 == 1);

                project.LeadCoordinator = coord;
                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection))
            {
                Assert.IsTrue(uow.Projects
                    .Include(p => p.LeadCoordinator)
                    .Single(p => p.Id == 2)
                    .LeadCoordinator.PartKey == coord.PartKey);
            }
        }

        [TestMethod]
        public void UpdateAssociatedEntityToNull()
        {
            Manager coordinator;
            using (var uow = new TestDbContext(_connection))
            {
                var project = uow.Projects
                    .Include(p => p.LeadCoordinator)
                    .Single(p => p.Id == 2);

                coordinator = project.LeadCoordinator;
                project.LeadCoordinator = null;

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection))
            {

                Assert.IsTrue(uow.Projects
                    .Include(p => p.LeadCoordinator)
                    .Single(p => p.Id == 2)
                    .LeadCoordinator == null);

                Assert.IsTrue(uow.Managers.Any(x => x.PartKey == coordinator.PartKey));
            }
        }

        #endregion

        #region ' Owned Entity '

        [TestMethod]
        public void UpdateOwnedEntityUpdateValues()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var company1 = uow.Companies
                    .Include(c => c.Address)
                    .Single(c => c.Id == 1);

                company1.Address.Street = "Route 66";

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection))
            {
                Assert.AreEqual("Route 66", uow.Companies.Include(p => p.Address).Single(p => p.Id == 1).Address.Street);
            }
        }

        [TestMethod]
        public void UpdateOwnedEntityNewEntity()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var company1 = uow.Companies
                    .Include(c => c.Address)
                    .Single(c => c.Id == 1);

                company1.IdAddress = 2;
                company1.Address = new CompanyAddress() {Id = 2, Street = "Route 66"};

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection))
            {
                Assert.AreEqual("Route 66", uow.Companies.Include(p => p.Address).Single(p => p.Id == 1).Address.Street);

                // PS: Faz esse teste, porque não faz sentido alterar a PK de um 1:1.
                Assert.AreEqual(1, uow.Companies.Include(p => p.Address).Single(p => p.Id == 1).Address.Id);
                Assert.AreNotEqual(2, uow.Companies.Include(p => p.Address).Single(p => p.Id == 1).Address.Id);

                Assert.AreEqual(1, uow.CompanyAddresses.Count());
            }
        }

        [TestMethod]
        public void UpdateOwnedEntityRemoveChildEntity()
        {

            using (var uow = new TestDbContext(_connection))
            {
                var company1 = uow.Companies
                    .Include(c => c.Address)
                    .Single(c => c.Id == 1);

                company1.Address = null;

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection))
            {

                Assert.IsTrue(uow.Companies
                    .Include(p => p.Address)
                    .Single(p => p.Id == 1)
                    .Address == null);

                Assert.IsFalse(uow.CompanyAddresses.Any());
            }
        }

        #endregion

        #region ' Associated Collection '

        [TestMethod]
        public void AssociatedCollectionAdd()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var project1 = uow.Projects
                    .Include(p => p.Stakeholders)
                    .Single(p => p.Id == 2);

                var company2 = uow.Companies.Single(p => p.Id == 2);

                project1.Stakeholders.Add(company2);

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection))
            {
                Assert.IsTrue(uow.Projects
                    .Include(p => p.Stakeholders)
                    .Single(p => p.Id == 2)
                    .Stakeholders.Count == 2);
            }
        }

        [TestMethod]
        public void AssociatedCollectionRemove()
        {
            Company company;
            using (var uow = new TestDbContext(_connection))
            {
                var project1 = uow.Projects
                    .Include(p => p.Stakeholders)
                    .Single(p => p.Id == 2);

                company = project1.Stakeholders.First();
                project1.Stakeholders.Remove(company);

                uow.Commit();
            }
            using (var uow = new TestDbContext(_connection))
            {
                Assert.IsTrue(uow.Projects
                    .Include(p => p.Stakeholders)
                    .Single(p => p.Id == 2)
                    .Stakeholders.Count == 0);

                Assert.IsTrue(uow.Companies.Any(p => p.Id == company.Id));
            }
        }

        #endregion

        #region ' Owned Collection '

        [TestMethod]
        public void OwnedCollectionUpdate()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var company1 = uow.Companies
                    .Include(p => p.Contacts)
                    .Single(p => p.Id == 2);

                company1.Name = "Company #1"; // Change from Company 1 to Company #1
                company1.Contacts.First().FirstName = "Bobby"; // change to bobby

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection))
            {
                var company = uow.Companies.Include(p => p.Contacts).Single(p => p.Id == 2);
                var contact = company.Contacts.First();

                Assert.AreEqual("Company #1", company.Name);
                Assert.AreEqual("Bobby", contact.FirstName);
                Assert.AreEqual("Jones", contact.LastName);
            }
        }

        [TestMethod]
        public void OwnedCollectionAdd()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var company1 = uow.Companies
                    .Include(p => p.Contacts.Select(m => m.Infos))
                    .Single(p => p.Id == 2);

                company1.Contacts.Add(new CompanyContact
                {
                    FirstName = "Charlie",
                    LastName = "Sheen",
                    Infos = new List<ContactInfo>
                    {
                        new ContactInfo {PhoneNumber = "123456789", Description = "Home"}
                    }
                });

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection))
            {
                var company = uow.Companies.Include(p => p.Contacts).Single(p => p.Id == 2);
                
                Assert.AreEqual(2, company.Contacts.Count);
                Assert.IsTrue(company.Contacts.Any(p => p.LastName == "Sheen"));
                Assert.IsTrue(company.Contacts.Single(p => p.LastName == "Sheen").Infos.Any(i => i.Description == "Home"));
                
            }
        }

        [TestMethod]
        public void OwnedCollectionAddMultiple()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var company1 = uow.Companies
                    .Include(p => p.Contacts.Select(m => m.Infos))
                    .Single(p => p.Id == 2);

                company1.Contacts.Add(new CompanyContact
                {
                    FirstName = "Charlie",
                    LastName = "Sheen",
                    Infos = new List<ContactInfo>
                    {
                        new ContactInfo {PhoneNumber = "123456789", Description = "Home"}
                    }
                });
                company1.Contacts.Add(new CompanyContact
                {
                    FirstName = "Tim",
                    LastName = "Sheen"
                });
                company1.Contacts.Add(new CompanyContact
                {
                    FirstName = "Emily",
                    LastName = "Sheen"
                });
                company1.Contacts.Add(new CompanyContact
                {
                    FirstName = "Mr",
                    LastName = "Sheen",
                    Infos = new List<ContactInfo>
                    {
                        new ContactInfo {PhoneNumber = "123456789", Description = "Home"}
                    }
                });
                company1.Contacts.Add(new CompanyContact
                {
                    FirstName = "Mr",
                    LastName = "X"
                });

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection))
            {
                var company = uow.Companies.Include(p => p.Contacts.Select(m => m.Infos)).Single(p => p.Id == 2);
                
                Assert.IsTrue(company.Contacts.Count == 6);
                Assert.IsTrue(company.Contacts.Any(c => c.FirstName == "Charlie"));
                Assert.IsTrue(company.Contacts.Single(c => c.FirstName == "Charlie").Infos.Any(i => i.Description == "Home"));
                Assert.IsTrue(company.Contacts.Any(c => c.FirstName == "Emily"));
                Assert.IsFalse(company.Contacts.Single(c => c.FirstName == "Emily").Infos.Any());
            }
        }

        [TestMethod]
        public void OwnedCollectionRemove()
        {
            using (var uow = new TestDbContext(_connection, new GraphdiffAggregateUpdateStrategy()))
            {
                var company1 = uow.Companies
                    .Include(p => p.Contacts.Select(m => m.Infos))
                    .Single(p => p.Id == 2);

                var contact = company1.Contacts.Single();
                company1.Contacts.Remove(contact);

                var repo = new CompanyRepository(uow);
                repo.AlterarAgregacao(company1, cfg => cfg.HasMany(c => c.Contacts));

                uow.Commit();
            }
            
            using (var uow = new TestDbContext(_connection))
            {
                var company = uow.Companies.Include(p => p.Contacts.Select(m => m.Infos)).Single(p => p.Id == 2);
                
                Assert.IsTrue(company.Contacts.Count == 0);

            }
        }

        [TestMethod]
        public void OwnedCollectionAddRemoveUpdate()
        {
            Company company2;

            using (var uow = new TestDbContext(_connection))
            {
                company2 = uow.Companies
                    .Include(p => p.Contacts.Select(m => m.Infos))
                    .Single(p => p.Id == 2);

                company2.Contacts.Add(new CompanyContact
                {
                    FirstName = "Hello", 
                    LastName = "Test",
                    Infos = new List<ContactInfo>
                    {
                        new ContactInfo {PhoneNumber = "123456789", Description = "Hello Home"}
                    }
                });

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection, new GraphdiffAggregateUpdateStrategy()))
            {
                company2 = uow.Companies
                    .Include(p => p.Contacts.Select(m => m.Infos))
                    .Single(p => p.Id == 2);

                // Update
                company2.Name = "Company #1"; // Change from Company 1 to Company #1
                company2.Contacts.First().FirstName = "Terrrrrry";

                // Remove
                var contatoARemover = company2.Contacts.Skip(1).First();
                Assert.IsTrue(contatoARemover.Infos.Any());
                company2.Contacts.Remove(contatoARemover);

                // add
                company2.Contacts.Add(new CompanyContact
                {
                    FirstName = "Charlie",
                    LastName = "Sheen",
                    Infos = new List<ContactInfo>
                    {
                        new ContactInfo {PhoneNumber = "123456789", Description = "Home"}
                    }
                });

                new CompanyRepository(uow).AlterarAgregacao(company2, cfg => cfg
                    .HasMany(p => p.Contacts, with => with
                        .HasMany(p => p.Infos)));

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection))
            {

                var test = uow.Companies
                    .Include(p => p.Contacts.Select(m => m.Infos))
                    .Single(p => p.Id == 2);

                Assert.AreEqual("Company #1", test.Name);
                Assert.AreEqual(2, test.Contacts.Count);
                Assert.AreEqual("Terrrrrry", test.Contacts.First().FirstName);
                Assert.AreEqual("Charlie", test.Contacts.Skip(1).First().FirstName);
                Assert.AreEqual("Home", test.Contacts.Skip(1).First().Infos.Single().Description);
            }
        }

        [TestMethod]
        public void OwnedCollectionWithOwnedCollection()
        {
            using (var uow = new TestDbContext(_connection))
            {
                var company1 = uow.Companies
                    .Include(p => p.Contacts.Select(m => m.Infos))
                    .First();

                company1.Contacts.First().Infos.First().Email = "testeremail";
                company1.Contacts.First().Infos.Add(new ContactInfo {Description = "Test", Email = "test@test.com"});

                uow.Commit();
            }

            using (var uow = new TestDbContext(_connection))
            {
                var value = uow.Companies.Include(p => p.Contacts.Select(m => m.Infos))
                    .First();

                Assert.AreEqual(2, value.Contacts.First().Infos.Count);
                Assert.AreEqual("testeremail", value.Contacts.First().Infos.First().Email);
            }
        }

        #endregion

        #region 2 way relation

        [TestMethod]
        public void EnsureWeCanUseCyclicRelationsOnOwnedCollections()
        {
            Manager manager;
            using (var uow = new TestDbContext(_connection))
            {
                manager = uow.Managers.Include(p => p.Employees).First();

                var newEmployee = new Employee {Key = "assdf", FirstName = "Test Employee", Manager = manager};
                manager.Employees.Add(newEmployee);

                uow.Commit();
            }
            using (var uow = new TestDbContext(_connection))
            {
                Assert.AreEqual(manager.FirstName, uow.Employees.Include(p => p.Manager).Single(p => p.Key == "assdf").Manager.FirstName);
            }
            using (var uow = new TestDbContext(_connection))
            {
                Assert.AreEqual("Test Employee", uow.Managers.Include(p => p.Employees).Single(p => p.PartKey == manager.PartKey).Employees.Single(p => p.Key == "assdf").FirstName);
            }
        }

        #endregion
    }
}
