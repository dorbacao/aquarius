using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Aquarius.Data.SqlClient.Tests.Modelo;
using Aquarius.Data.SqlClient.Tests.UnitOfWork;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aquarius.Data.SqlClient.Tests.Testes
{
    [TestClass]
    public class SqlBulkCopyTests
    {

        const string TempDatabaseConnectionStringName = "TestDatabase";

        [ClassInitialize]
        public static void ClassInitialize(TestContext currentTestContext)
        {
            // Configura 'DataDirectory' (usado na connection string) para apontar para o arquivo.mdf localizado no diretório onde 
            // foi realizado do deploy dos arquivos do teste.
            AppDomain.CurrentDomain.SetData("DataDirectory", currentTestContext.TestDeploymentDir);

            // verifica se a string de conexão está devidamente configurada no App.config.
            var connectionStringConfig = System.Configuration.ConfigurationManager.ConnectionStrings[TempDatabaseConnectionStringName];
            Assert.IsNotNull(connectionStringConfig, String.Format("Não foi possível localizar a string de conexão '{0}' no arquivo de configuração.", TempDatabaseConnectionStringName));
        }

        [TestMethod]
        public void Realiza_BulkInsert_Com_DataAnnotations()
        {

            // cria lista de itens a serem incluidos.
            var listaEntidades = (from i in Enumerable.Range(1, 10000)
                                  select new Modelo.EntidadeTesteDataAnnotation() {
                                      Id = i + 10000, 
                                      Nome = String.Format("Nome: {0}", i), 
                                      Idade = i,
                                      TipoPessoa = TipoPessoa.Juridica,
                                  }).ToList();
                       

            // Para o sucesso na verificação do teste, verifica se a entidade possui TableAttributes.
            Verifica_Se_A_Entidade_Possui_MappingAttributes(
                listaEntidades.GetType().GetGenericArguments().First()
            );


            using (var uow = new MainUnitOfWork(TempDatabaseConnectionStringName))
            {
                // Cria BD.
                if (!uow.Database.Exists()) uow.Database.Initialize(force: false);

                var repo = new Repository<EntidadeTesteDataAnnotation>(uow);
                var qtdOriginalDeRegistrosNaTabela = repo.Contar();

                // realiza bulk insert.
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[TempDatabaseConnectionStringName].ConnectionString;

                var bulkobj = new SqlBulkCopy<EntidadeTesteDataAnnotation>();
                bulkobj.BulkInsert(connectionString, listaEntidades);

                // verifica a quantidade de itens inseridos
                var qtdEntidadesCadastradas = repo.Contar();
                Assert.IsTrue(qtdEntidadesCadastradas == listaEntidades.Count() + qtdOriginalDeRegistrosNaTabela, "Não adicionou os registros na tabela.");
            }

        }

        [TestMethod]
        public void Realiza_BulkInsert_Sem_DataAnnotations()
        {

            // cria lista de itens a serem incluidos.
            var listaEntidades = (from i in Enumerable.Range(1, 10000)
                                  select new Modelo.EntidadeTeste()
                                  {
                                      Id = i + 10000,
                                      Nome = String.Format("Nome: {0}", i),
                                      Idade = i,
                                      TipoPessoa = TipoPessoa.Juridica,
                                  }).ToList();


            using (var uow = new MainUnitOfWork(TempDatabaseConnectionStringName))
            {
                // Cria BD.
                if (!uow.Database.Exists()) uow.Database.Initialize(force: false);

                var repo = new Repository<EntidadeTeste>(uow);
                var qtdOriginalDeRegistrosNaTabela = repo.Contar();

                // realiza bulk insert.
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[TempDatabaseConnectionStringName].ConnectionString;

                var bulkobj = new SqlBulkCopy<EntidadeTeste>();
                bulkobj.NomeTabela = "TabelaEntidadesTeste";
                bulkobj.MapColumn(p => p.Nome, "NomeEntidade");
                bulkobj.MapColumn(p => p.TipoPessoa, "TipoDePessoa");
                bulkobj.Ignore(p => p.Idade);
                bulkobj.BulkInsert(connectionString, listaEntidades);

                // verifica a quantidade de itens inseridos
                var qtdEntidadesCadastradas = repo.Contar();
                Assert.IsTrue(qtdEntidadesCadastradas == listaEntidades.Count() + qtdOriginalDeRegistrosNaTabela, "Não adicionou os registros na tabela.");

            }

        }

        [TestMethod]
        public void Realiza_Insercao_Com_Repository()
        {
            // cria lista de itens a serem incluidos.
            var listaEntidades = Enumerable.Range(1, 1000).Select(id => new Modelo.EntidadeTeste() 
            { 
                Nome = id.ToString(), TipoPessoa = TipoPessoa.Fisica
            }).ToList();

            int? qtdOriginalDeRegistrosNaTabela = null;

            using (var uow = new MainUnitOfWork(TempDatabaseConnectionStringName))
            {
                var repo = new Repository<EntidadeTeste>(uow);
                qtdOriginalDeRegistrosNaTabela = repo.Contar();

                listaEntidades.ForEach(repo.Incluir);
                uow.Commit();
            }

            // verifica a quantidade de itens inseridos
            using (var uow = new MainUnitOfWork(TempDatabaseConnectionStringName))
            {
                //uow.Database.Initialize(false);
                var qtdEntidadesCadastradas = new Repository<EntidadeTeste>(uow).Contar();
                Assert.IsTrue(qtdEntidadesCadastradas == listaEntidades.Count() + qtdOriginalDeRegistrosNaTabela.Value, "Não adicionou os registros na tabela.");
            }

        }

        protected void Verifica_Se_A_Entidade_Possui_MappingAttributes(Type typeToCheck)
        {
            // Valida existência do TableAttribute
            var entidadePossuiTableAttribute = typeToCheck.GetCustomAttributes(typeof(TableAttribute), false).Any();
            Assert.IsTrue(entidadePossuiTableAttribute, String.Format("A entidade '{0}' utilizada neste Test Method não possui TableAttribute definido.", typeToCheck.Name));

            var todosAtributosUsados = from prop in typeToCheck.GetProperties()
                                       from attr in prop.GetCustomAttributes(false)
                                       select attr;


            // Valida existência de alguma propriedade com ColumnAttribute.
            var entidadePoussuiColumnAttribute = todosAtributosUsados.Any(attr => attr is ColumnAttribute);
            Assert.IsTrue(entidadePoussuiColumnAttribute, String.Format("A entidade '{0}' utilizada neste Test Method não possui nenhuma propriedade com ColumnAttribute definido.", typeToCheck.Name));
        }

    }
}
