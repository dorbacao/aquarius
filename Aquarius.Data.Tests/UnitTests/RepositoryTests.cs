using System;
using System.Collections.Generic;
using System.Linq;
using Aquarius.Data.Tests.Criterios.Estado;
using Aquarius.Data.Tests.Criterios.Pais;
using Aquarius.Data.Tests.Data;
using Aquarius.Data.Tests.Modelo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Aquarius.Data.Tests.UnitTests
{

    [TestClass]
    public class RepositoryTests
    {
        protected IUnitOfWork MainUnitOfWork;
        protected CidadeReadonlyRepository CidadeRepository;
        protected EstadoReadonlyRepository EstadoRepository;
        protected PaisReadonlyRepository PaisRepository;


        public void TestInitialize(Type uowType)
        {
            throw new NotImplementedException("A implementação desta Classe de Testes está errada.");
            
            // Cria UoW
            this.MainUnitOfWork = Activator.CreateInstance(uowType) as IUnitOfWork;
            Assert.IsNotNull(this.MainUnitOfWork);

            // Cria, se necessário, caso de teste.
            // No caso do Entity Framework, considera o BD como criado.
            // ToDo: alterar para não considerar o BD como criado no caso do EntityFramework.
            if (this.MainUnitOfWork is UnitOfWorkMemory)
            {
                // Cria dados para testes
                var listaPais = SeedPais();
                var listaEstado = SeedEstado(listaPais);
                var listaCidade = SeedCidade(listaEstado);

                listaPais.ForEach(MainUnitOfWork.RegisterClean);
                listaEstado.ForEach(MainUnitOfWork.RegisterClean);
                listaCidade.ForEach(MainUnitOfWork.RegisterClean);
                this.MainUnitOfWork.Commit();
            }


            // Cria repositorios
            this.CidadeRepository = new CidadeReadonlyRepository(this.MainUnitOfWork);
            this.EstadoRepository = new EstadoReadonlyRepository(this.MainUnitOfWork);
            this.PaisRepository = new PaisReadonlyRepository(this.MainUnitOfWork);

        }

        #region ' Seed Methods '

        public List<Pais> SeedPais()
        {
            var brasil = new Pais() {Id = 1, NomePais = "Brasil" };
            var chile = new Pais() {Id = 2, NomePais = "Chile" };
            var eua = new Pais() {Id = 3, NomePais = "Estados Unidos da America" };

            return new List<Pais>() {brasil, chile, eua};
        }

        public List<Estado> SeedEstado(IEnumerable<Pais> listaPais)
        {
            var brasil = listaPais.Single(p => p.NomePais == "Brasil");

            var listaEstado = new List<Estado>()
                {
                    new Estado() {Id = 1, UF = "ES", NomeEstado = "Espirito Santo", Pais = brasil},
                    new Estado() {Id = 2, UF = "MG", NomeEstado = "Minas Gerais", Pais = brasil},
                    new Estado() {Id = 3, UF = "RJ", NomeEstado = "Rio de Janeiro", Pais = brasil},
                    new Estado() {Id = 4, UF = "RN", NomeEstado = "Rio Grande do Norte", Pais = brasil},
                    new Estado() {Id = 5, UF = "RS", NomeEstado = "Rio Grande do Sul", Pais = brasil},
                    new Estado() {Id = 6, UF = "SE", NomeEstado = "Sergipe", Pais = brasil},
                    new Estado() {Id = 7, UF = "SP", NomeEstado = "São Paulo", Pais = brasil},
                };

            brasil.Estados = listaEstado;
            return listaEstado;

        }

        public List<Cidade> SeedCidade(IEnumerable<Estado> listaEstado)
        {
            var rj = listaEstado.Single(e => e.UF == "RJ");
            var es = listaEstado.Single(e => e.UF == "ES");

            var listaCidade = new List<Cidade>()
                {
                    //new Cidade() {Id = 1, NomeCidade = "Niterói", Estado = rj},
                    //new Cidade() {Id = 2, NomeCidade = "Vila Velha", Estado = es}
                };
            listaCidade.ForEach(c => c.Estado.Cidades.Add(c));

            return listaCidade;
        }

        #endregion


        [TestMethod]
        [TestCase(typeof(UnitOfWorkMemory))]
        [TestCase(typeof(MainUnitOfWork))]
        public void Contando(Type uowType)
        {
            TestInitialize(uowType);

            Assert.IsTrue(EstadoRepository.Contar() > 0);
        }


        [TestMethod]
        [TestCase(typeof(UnitOfWorkMemory))]
        [TestCase(typeof(MainUnitOfWork))]
        public void Selecionando_ComCustomOutput(Type uowType)
        {
            TestInitialize(uowType);


            var rioDeJaneiro = EstadoRepository.Selecionar(
                output: e => e.NomeEstado, 
                where: e => e.UF == "RJ");
            Assert.IsTrue(rioDeJaneiro == "Rio de Janeiro", "Não selecionou item corretamente");
        }

        [TestMethod]
        [TestCase(typeof(UnitOfWorkMemory))]
        [TestCase(typeof(MainUnitOfWork))]
        public void Selecionando_ComJoin(Type uowType)
        {
            TestInitialize(uowType);

            var estado = EstadoRepository.Selecionar(
                where: e => e.UF == "RJ",
                joinWith: new string[] {"Pais"});
            Assert.IsTrue(estado.NomeEstado == "Rio de Janeiro", "Não selecionou item corretamente");
            Assert.IsNotNull(estado.Pais, "Não realizou joinWith");
        }


        [TestMethod]
        [TestCase(typeof(UnitOfWorkMemory))]
        [TestCase(typeof(MainUnitOfWork))]
        public void Listando(Type uowType)
        {
            TestInitialize(uowType);

            var listaUf = EstadoRepository.Listar(
                where: e => e.NomeEstado.Contains("ir"),
                orderBy: l => l.OrderByDescending(i=> i.UF),
                output: e => e.UF).ToList();
            Assert.IsTrue(listaUf.Count == 2, "Não filtrou corretamente");
            Assert.IsTrue(String.Compare(listaUf[0], listaUf[1], StringComparison.CurrentCulture) > 0, "Não ordenou corretamene" );
        }

        [TestMethod]
        [TestCase(typeof(UnitOfWorkMemory))]
        //[TestCase(typeof(MainUnitOfWork))]
        public void LevantaErroAoInformarNavigationPropertyInvalido(Type uowType)
        {
            TestInitialize(uowType);


            try
            {
                var niteroi = CidadeRepository.Selecionar(c => c.NomeCidade == "Niterói", joinWith: new[] { "Estado.Cidades.Estado.InvalidNavigationProperty", "Estado.Pais" });
                Assert.Fail("Não levantou erro ao fazer referência a um Navigation Property inválido.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is InvalidNavigationPropertyException, ex.Message);
                Assert.IsTrue((ex as InvalidNavigationPropertyException).InvalidNavigationProperty == "InvalidNavigationProperty");
                Assert.IsTrue((ex as InvalidNavigationPropertyException).Type == typeof(Modelo.Estado));
            }

            
        }

        [TestMethod]
        [TestCase(typeof(UnitOfWorkMemory))]
        [TestCase(typeof(MainUnitOfWork))]
        public void ListarUsandoCriterio(Type uowType)
        {
            TestInitialize(uowType);


            // Teste utilizando um criterio que herde de 'Criteria' (classe concreta)
            var listaDePais = PaisRepository.Listar(new QueComecemComALetra('B'));
            Assert.IsTrue(listaDePais.All(p => p.NomePais[0] == 'B'));

            // Teste utilizando um crite´rio que implementa 'ICriteria<TEntidade>'.
            var listaEstados = EstadoRepository.Listar(new DoPais("Brasil"));
            Assert.IsTrue(listaEstados.All(e => e.Pais.NomePais == "Brasil"));

        }


        public class IdsDasCidades : Criteria.ICriteria<Cidade, int>
        {
            public IQueryable<int> MeetCriteria(IQueryable<Cidade> query)
            {
                //return query.Select(c => c.Id);
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        [TestCase(typeof(UnitOfWorkMemory))]
        public void TestarMax(Type uowType)
        {
            TestInitialize(uowType);

            var objComMaxId = CidadeRepository.Max(new IdsDasCidades(), i => new ApenasNomeEId(){  Id = i} );
            Assert.IsTrue(objComMaxId.Id == 2);
        }

        public class CidadesQueComecaoCom : Criteria.ICriteria<Cidade>
        {
            private string ComecemCom { get; set; }
            public CidadesQueComecaoCom(string comecemCom)
            {
                ComecemCom = comecemCom;
            }
            public IQueryable<Cidade> MeetCriteria(IQueryable<Cidade> query)
            {
                return query.Where(p => p.NomeCidade.StartsWith(ComecemCom));
            }
        }

        public class ApenasNomeEId : IComparable
        {
            public int Id { get; set; }
            public string Nome { get; set; }

            public int CompareTo(object other)
            {
                var obj = other as ApenasNomeEId;
                if (obj == null) return 1;
                else if (this.Id < obj.Id) return -1;
                else if (this.Id > obj.Id) return +1;
                else return 0;
            }
        }

        [TestMethod]
        [TestCase(typeof(UnitOfWorkMemory))]
        public void TestarListarComCritiriaEOutput(Type uowType)
        {
            TestInitialize(uowType);

            //var listagem = CidadeRepository.Listar(
            //    new CidadesQueComecaoCom("V"), cidade => new ApenasNomeEId()
            //                                                 {
            //                                                     Id = cidade.Id,
            //                                                     Nome = cidade.NomeCidade
            //                                                 }
            //    ).ToList();

            //Assert.IsTrue(listagem.Count > 0);
        }

        [TestMethod]
        [TestCase(typeof(UnitOfWorkMemory))]
        public void TestarSelecionarComCritiriaEOutput(Type uowType)
        {
            TestInitialize(uowType);

            //var listagem = CidadeRepository.Selecionar(
            //    new CidadesQueComecaoCom("V"), cidade => new ApenasNomeEId()
            //    {
            //        Id = cidade.Id ,
            //        Nome = cidade.NomeCidade
            //    }
            //);

            //Assert.IsNotNull(listagem.Nome);
        }

        [TestMethod]
        [TestCase(typeof(UnitOfWorkMemory))]
        public void TestarMaxComCritiriaEOutput(Type uowType)
        {
            TestInitialize(uowType);

            //var listagem = CidadeRepository.Selecionar(
            //    new CidadesQueComecaoCom("V"), cidade => new ApenasNomeEId()
            //    {
            //        Id = cidade.Id,
            //        Nome = cidade.NomeCidade
            //    }
            //);

            //Assert.IsNotNull(listagem.Nome);
        }

      

    }

}
