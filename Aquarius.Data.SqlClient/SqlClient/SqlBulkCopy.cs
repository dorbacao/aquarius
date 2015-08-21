using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Aquarius.Data.SqlClient.SqlClient
{
    /// <summary>
    /// Classe quer permite o carregamento em massa de dados para uma tabela no SQL Server.
    /// </summary>
    public class SqlBulkCopy<T> where T : class
    {
        #region Proriedades
        public string NomeTabela { get; set; }

        private Dictionary<string, string> mapeados;
        private List<string> ignorados;

        public Dictionary<string, string> Mapeados { get { return this.mapeados; } }
        public List<string> Ignorados { get { return this.ignorados; } }
        #endregion

        #region Construtores
        public SqlBulkCopy()
        {
            this.mapeados = new Dictionary<string, string>();
            this.ignorados = new List<string>();
        }
        #endregion

        #region Métodos Mapeamento
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPropriedade"></typeparam>
        /// <param name="propertySelector"></param>
        /// <param name="columnName"></param>
        public void MapColumn<TPropriedade>(Expression<Func<T, TPropriedade>> propertySelector, string columnName)
        {
            var expression = (MemberExpression)propertySelector.Body;
            string name = expression.Member.Name;

            if (!Ignorados.Any(p => p == name))
                this.Mapeados.Add(name, columnName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPropriedade"></typeparam>
        /// <param name="propertySelector"></param>
        public void Ignore<TPropriedade>(Expression<Func<T, TPropriedade>> propertySelector)
        {
            var expression = (MemberExpression)propertySelector.Body;
            string name = expression.Member.Name;

            if (Mapeados.Any(p => p.Key == name))
                this.Mapeados.Remove(name);

            this.Ignorados.Add(name);
        }        
        #endregion

        #region Métodos Data Annotation
        /// <summary>
        ///  Determina o nome da tabela a partir das decorações da classe (DataAnnotations).
        /// </summary>
        /// <remarks>
        /// Este método é uma repetição do método já existente em <c>Vvs.Data.Mapping.Functions</c>.
        /// </remarks>
        protected static String GetDefaultTableName()
        {
            var typeOfT = typeof(T);
            var tableAttribute = (TableAttribute)typeOfT.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault();            
            var tableName = tableAttribute != null ? (tableAttribute.Name ?? typeOfT.Name) : typeOfT.Name;
            //
            return tableName;
        }

        /// <summary>
        /// Determina o nome da coluna no banco de dados para uma determinada propriedade de uma classe
        /// a partir das decorações (DataAnnotations).
        /// </summary>
        /// <remarks>
        /// Este método é uma repetição do método já existente em <c>Vvs.Data.Mapping.Functions</c>.
        /// </remarks>
        protected static String GetDefaultColumnName(PropertyDescriptor prop)
        {
            var columnAttribute = (ColumnAttribute)prop.ComponentType.GetProperty(prop.Name)
                                                        .GetCustomAttributes(typeof(ColumnAttribute), false)
                                                        .FirstOrDefault();
            var columnName = columnAttribute != null ? (columnAttribute.Name ?? prop.Name) : prop.Name;           
            return columnName;
        }
        #endregion

        #region BulkInsert(...)

        public void BulkInsert(String connectionString, IList<T> listaObjetosAIncluir)
        {
            var tableName = this.NomeTabela ?? GetDefaultTableName();
            BulkInsert(connectionString, tableName, listaObjetosAIncluir);
        }

        public void BulkInsert(string connectionString, string nomeTabela, IList<T> listaObjetosAIncluir)
        {
            var dataTable = new DataTable();

            // Specifications
            var ehTipoPrimitivo = new Func<PropertyDescriptor, bool>((prop) => prop.PropertyType.Namespace != null && prop.PropertyType.Namespace.Equals("System"));    // Não é possivel utilizar 'Type.IsPrimitive' porque para o caso da String o atributo é False.
            var ehTipoEnum = new Func<PropertyDescriptor, bool>((prop) => prop.PropertyType.Namespace != null && prop.PropertyType.IsEnum);
            var contemNotMappedAttribute = new Func<PropertyDescriptor, bool>((prop) => prop.ComponentType.GetProperty(prop.Name).GetCustomAttributes(typeof(NotMappedAttribute), false).Any());
            var ehIgnorado = new Func<PropertyDescriptor, bool>((prop) => ignorados.Any(p => p == prop.Name));
            var ehMapeado = new Func<PropertyDescriptor, bool>((prop) => mapeados.Any(p => p.Key == prop.Name));


            // Seleciona os atributos da classe que serão mapeadas no Bulk Insert.
            var propriedades = TypeDescriptor.GetProperties(typeof(T))
                                             .Cast<PropertyDescriptor>()
                                             .Where(prop => ehTipoPrimitivo(prop) || ehTipoEnum(prop))
                                             .Where(prop => !contemNotMappedAttribute(prop) && !ehIgnorado(prop))
                                             .ToList();
            
            // Mapeia as colunas que não foram mapeadas.
            foreach (var prop in propriedades.Where(prop => !ehMapeado(prop)))
                mapeados.Add(prop.Name, GetDefaultColumnName(prop));

            //
            // Cria COLUNAS no DataTable.
            var colunas = (from prop in propriedades
                           let nomeColunaBD = mapeados[prop.Name]
                           select new DataColumn(nomeColunaBD, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType)
                           ).ToArray();

            dataTable.Columns.AddRange(colunas);

            //
            // Cria LINHAS no DataTable
            foreach (var item in listaObjetosAIncluir)
            {
                var dataRow = dataTable.NewRow();
                propriedades.ForEach(prop => dataRow[mapeados[prop.Name]] = prop.GetValue(item) ?? DBNull.Value);
                dataTable.Rows.Add(dataRow);
            }         

            BulkInsert(connectionString, nomeTabela, dataTable);
        }

        public void BulkInsert(string connectionString, string nomeTabela, DataTable dataTable)
        {

            using (var bulkCopy = new System.Data.SqlClient.SqlBulkCopy(connectionString, SqlBulkCopyOptions.KeepIdentity))
            {
                bulkCopy.BatchSize = dataTable.Rows.Count;
                bulkCopy.DestinationTableName = nomeTabela;
                bulkCopy.BulkCopyTimeout = 0;

                foreach (DataColumn col in dataTable.Columns) 
                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                
                bulkCopy.WriteToServer(dataTable);
            }
        }
       
        #endregion
    }
}
