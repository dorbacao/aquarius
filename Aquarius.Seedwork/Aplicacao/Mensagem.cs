using System;
using System.Runtime.Serialization;

namespace Vvs.Domain.Seedwork.Aplicacao
{
    [DataContract]
    public class Mensagem
    {
        #region ' Constructor '

        private Mensagem() { }

        public Mensagem(string texto, TipoMensagem tipo)
        {
            if (texto == null) throw new ArgumentNullException("texto");

            this.Texto = texto;
            this.Tipo = tipo;
        }

        #endregion

        [DataMember]
        public string Texto { get; private set; }
        
        [DataMember]
        public TipoMensagem Tipo { get; private set; }
    }
}
