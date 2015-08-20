using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vvs.Domain.Seedwork;

namespace Northwind.Tests.Domain.Entity
{
    public class Cliente : Pessoa
    {
        public string Cpf { get; set; }
    }
}
