using System;
using System.Collections.Generic;
using System.Linq;

namespace Aquarius.Seedwork.Aplicacao
{
    public class VoidResponse : IResponse
    {

        #region ' Members '

        private readonly List<Mensagem> _mensagens;

        #endregion

        #region [ Constructor ]

        public VoidResponse()
        {
            this._mensagens = new List<Mensagem>();
        }

        public VoidResponse(IEnumerable<Mensagem> mensagens)
        {
            this._mensagens = mensagens != null ? mensagens.ToList() : new List<Mensagem>();
        }

        public VoidResponse(params Mensagem[] mensagens) : this((IEnumerable<Mensagem>)mensagens)
        {
            if (mensagens == null) throw new ArgumentNullException("mensagens");
            if (mensagens.Length == 0) throw new ArgumentException("Nenhuma mensagem informada");
        }

        #endregion

        public IEnumerable<Mensagem> Mensagens
        {
            get { return this._mensagens; }
        }

        public VoidResponse AddMensagem(TipoMensagem tipo, string texto)
        {
            if (texto == null) throw new ArgumentNullException("texto");
            _mensagens.Add(new Mensagem(texto, tipo));
            return this;
        }

        #region ' Status '

        public bool Ok
        {
            get { return !HasError; }
        }

        public bool HasAlerta
        {
            get { return Mensagens.Any(msg => msg.Tipo == TipoMensagem.Alerta); }
        }

        public bool HasError
        {
            get { return Mensagens.Any(msg => msg.Tipo == TipoMensagem.Erro || msg.Tipo == TipoMensagem.ErroCritico); }
        }

        public bool HasInfo
        {
            get { return Mensagens.Any(msg => msg.Tipo == TipoMensagem.Info); }
        }

        #endregion

    }
}
