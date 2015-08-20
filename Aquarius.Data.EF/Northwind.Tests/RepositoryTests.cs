using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Northwind.Tests.Domain.Entity;
using Northwind.Tests.Domain.Repositorios;
using Northwind.Tests.UnitOfWork;

namespace Northwind.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        protected DbConnection Connection;

        #region ' Test Initialize/SeedData/CleanUp '

        [TestInitialize]
        public void TestInitialize()
        {
            Connection = Effort.DbConnectionFactory.CreateTransient();
            SeedData();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Connection != null) Connection.Dispose();
        }

        public void SeedData()
        {
            var territories = new List<Territories>
            {
                new Territories { TerritoryDescription = "Territorio Teste"}, 
                new Territories { TerritoryDescription = "Outro Território Teste"}
            };
            territories.ForEach(t => t.GenerateNewIdentity());

            var region = new Region
            {
                RegionDescription = "Região de Teste",
                Territories = territories
            };
            region.GenerateNewIdentity();

            using (var uow = CreateTransientUnitOfWork())
            {
                var regionRepository = new RegionRepository(uow);

                regionRepository.Incluir(region);
                uow.Commit();

            }
        }

        protected MainUnitOfWork CreateTransientUnitOfWork()
        {
            return new MainUnitOfWork(Connection, false);
        }

        #endregion

        [TestMethod]
        public void ExcluirTerritoriosDeUmaRegiao()
        {
            using (var uow = CreateTransientUnitOfWork())
            {
                var regionRepository = new RegionRepository(uow);
                var territoriesRepository = new TerritoriesRepository(uow);

                var region = regionRepository.Listar().FirstOrDefault();
                var qtdOriginalDeRegistrosNaTabelaTerritories = territoriesRepository.Contar();

                if (region != null)
                {
                    var qtdTerritoriesRegion = region.Territories.Count;

                    region.Territories.ToList().ForEach(territoriesRepository.Excluir);
                    uow.Commit();
                    Assert.AreEqual(qtdOriginalDeRegistrosNaTabelaTerritories - qtdTerritoriesRegion
                        , territoriesRepository.Contar());
                }
                else
                {
                    uow.Rollback();
                    Assert.IsTrue(false);
                }
            }
        }

        [TestMethod]
        public void AtualizarTerritoriosDeUmaRegiao()
        {
            using (var uow = CreateTransientUnitOfWork())
            {
                var regionRepository = new RegionRepository(uow);
                var region = regionRepository.Listar().FirstOrDefault();

                if (region != null)
                {
                    region.Territories.First().TerritoryDescription = "Território de Teste Novo";
                    regionRepository.Alterar(region);
                    uow.Commit();
                    Assert.AreEqual(region.Territories.First().TerritoryDescription
                        , "Território de Teste Novo");
                }
                else
                {
                    uow.Rollback();
                    Assert.IsTrue(false);
                }
            }
        }

        [TestMethod]
        public void IncluirAtualizarExcluirTerritorioDeUmaRegiao()
        {
            using (var uow = CreateTransientUnitOfWork())
            {
                var regionRepository = new RegionRepository(uow);
                var region = regionRepository.Listar().FirstOrDefault(r => r.Territories.Count >= 2);

                if (region != null)
                {
                    var territories = new Territories {TerritoryDescription = "Território de Teste Novo"};
                    territories.GenerateNewIdentity();

                    //var Territories = region.Territories.Last();

                    region.Territories.First().TerritoryDescription = "Território de Teste";
                    //region.Territories.Remove(Territories);
                    region.Territories.Add(territories);

                    regionRepository.Alterar(region);
                    uow.Commit();

                    Assert.AreEqual(region.Territories.First().TerritoryDescription
                        , "Território de Teste");
                    Assert.AreEqual(region.Territories.Last().TerritoryDescription
                        , "Território de Teste Novo");
                }
                else
                {
                    uow.Rollback();
                    Assert.IsTrue(false);
                }
            }
        }

        [TestMethod]
        public void ExcluirUmaRegiaoComSeusTerritorios()
        {
            using (var uow = CreateTransientUnitOfWork())
            {
                var regionRepository = new RegionRepository(uow);
                var territoriesRepository = new TerritoriesRepository(uow);

                var region = regionRepository.Listar().FirstOrDefault(r => r.Territories.Count >= 2);

                if (region != null)
                {
                    var id = region.Territories.First().Id;
                    region.Territories.ToList().ForEach(territoriesRepository.Excluir);
                    regionRepository.Excluir(region);
                    uow.Commit();

                    Assert.IsNull(territoriesRepository.Selecionar(t => t.Id == id));
                }
                else
                {
                    uow.Rollback();
                    Assert.IsTrue(false);
                }
            }
        }

        [TestMethod]
        public void IncluirRegiao()
        {
            using (var uow = CreateTransientUnitOfWork())
            {
                var region = new Region { RegionDescription = "Região de Teste" };
                region.GenerateNewIdentity();

                var regionRepository = new RegionRepository(uow);
                var qtdOriginalDeRegistrosNaTabelaRegion = regionRepository.Contar();

                regionRepository.Incluir(region);
                uow.Commit();

                Assert.AreEqual(qtdOriginalDeRegistrosNaTabelaRegion + 1, regionRepository.Contar());
            }
        }

        [TestMethod]
        public void IncluirRegiaoComTerritorios()
        {
            var territorieses = new List<Territories>
            {
                new Territories { TerritoryDescription = "Territorio Teste"}, 
                new Territories { TerritoryDescription = "Outro Território Teste"}
            };

            territorieses.ForEach(t => t.GenerateNewIdentity());

            var region = new Region
            {
                RegionDescription = "Região de Teste",
                Territories = territorieses
            };

            region.GenerateNewIdentity();

            using (var uow = CreateTransientUnitOfWork())
            {
                var regionRepository = new RegionRepository(uow);
                var terrotoriesRepository = new TerritoriesRepository(uow);

                var qtdOriginalDeRegistrosNaTabelaRegion = regionRepository.Contar();
                var qtdOriginalDeRegistrosNaTabelaTerritories = terrotoriesRepository.Contar();

                regionRepository.Incluir(region);
                uow.Commit();

                Assert.AreEqual(qtdOriginalDeRegistrosNaTabelaRegion + 1, regionRepository.Contar());
                Assert.AreEqual(qtdOriginalDeRegistrosNaTabelaTerritories + territorieses.Count,
                    terrotoriesRepository.Contar());
            }
        }

        [TestMethod]
        public void IncluirTerritorioDeUmaRegiao()
        {
            using (var uow = CreateTransientUnitOfWork())
            {
                var region = new Region { RegionDescription = "Região de Teste" };
                region.GenerateNewIdentity();

                var regionRepository = new RegionRepository(uow);

                regionRepository.Incluir(region);
                uow.Commit();
            }

            using (var uow = CreateTransientUnitOfWork())
            {
                var territoriesRepository = new TerritoriesRepository(uow);
                var regionRepository = new RegionRepository(uow);

                var region = regionRepository.Listar().FirstOrDefault(r => r.Territories.Count.Equals(0));
                var qtdOriginalDeRegistrosNaTabelaTerritories = territoriesRepository.Contar();

                if (region != null)
                {
                    var territories = new Territories { TerritoryDescription = "Território de Teste" };
                    territories.GenerateNewIdentity();

                    region.Territories = (new List<Territories> { territories });
                    regionRepository.Alterar(region);
                    uow.Commit();

                    Assert.AreEqual(qtdOriginalDeRegistrosNaTabelaTerritories + 1
                        , territoriesRepository.Contar());
                    Assert.IsNotNull(region.Territories);
                }
                else
                {
                    uow.Rollback();
                    Assert.IsTrue(false);
                }
            }
        }

        [TestMethod]
        public void Attached_Update()
        {
            Region region;

            using (var uow = CreateTransientUnitOfWork())
            {
                // Seleciona
                var regionRepository = new RegionRepository(uow);
                region = regionRepository.Listar().First();
                
                // Altera
                region.RegionDescription = Guid.NewGuid().ToString();
                regionRepository.Alterar(region);
                uow.Commit();
            }

            using (var uow = CreateTransientUnitOfWork())
            {
                var regionRepository = new RegionRepository(uow);
                Assert.IsNotNull(regionRepository.Selecionar(r => r.Id == region.Id && r.RegionDescription == region.RegionDescription));
            }
        }

        [TestMethod]
        public void Dettached_Update()
        {
            Region region;

            // Seleciona
            using (var uow = CreateTransientUnitOfWork())
            {
                var regionRepository = new RegionRepository(uow);
                region = regionRepository.Listar().FirstOrDefault();
            } // Simulate Dettach


            // Altera
            using (var uow = CreateTransientUnitOfWork())
            {
                region.RegionDescription += Guid.NewGuid().ToString();
                var regionRepository = new RegionRepository(uow);                
                regionRepository.Alterar(region);
                uow.Commit();
            }

            // Valida alteração
            using (var uow = CreateTransientUnitOfWork())
            {
                var regionRepository = new RegionRepository(uow);
                Assert.IsNotNull(regionRepository.Selecionar(r => r.Id == region.Id && r.RegionDescription == region.RegionDescription));
            }
        }

        [TestMethod]
        public void Detached_UpdateWhenKeyExistsInContext()
        {
            Region dettachedRegion;

            using (var uow = CreateTransientUnitOfWork())
            {

                var regionRepository = new RegionRepository(uow);
                var originalRegion = regionRepository.Listar().First();

                // Simulate Dettached
                dettachedRegion = new Region();
                dettachedRegion.ChangeCurrentIdentity(originalRegion.Id);

                // Altera
                dettachedRegion.RegionDescription = Guid.NewGuid().ToString();
                regionRepository.Alterar(dettachedRegion);
                uow.Commit();
            }

            using (var uow = CreateTransientUnitOfWork())
            {
                var regionRepository = new RegionRepository(uow);
                Assert.IsTrue(regionRepository.Existe(r => r.Id == dettachedRegion.Id && r.RegionDescription == dettachedRegion.RegionDescription));
            }

        }

    }
}
