using System;
using System.Collections.Generic;
using System.Linq;
using Vvs.Domain.Seedwork.Aplicacao.Extensions;

namespace Vvs.Domain.Seedwork.Aplicacao
{
    /// <summary>
    ///     Helper Class para criação de Responses.
    /// </summary>
    public static class Response
    {
        public static VoidResponse ComAlerta(string mensagem, params string[] outrasMensagens)
        {
            var mensagens = (new [] { mensagem }).Concat(outrasMensagens).ToList();

            var response = new VoidResponse();
            mensagens.ForEach(msg => response.AddAlerta(msg));
            return response;
        }

        public static Response<T> ComAlerta<T>(T value, string mensagem, params string[] outrasMensagens)
        {
            var mensagens = (new[] { mensagem }).Concat(outrasMensagens).ToList();

            var response = new Response<T>(value);
            mensagens.ForEach(msg => response.AddAlerta(msg));
            return response;
        }

        public static VoidResponse Vazio()
        {
            return new VoidResponse();
        }

        public static Response<T> Vazio<T>()
        {
            return new Response<T>();
        }

        public static VoidResponse Erro(string mensagem, params string[] outrasMensagens)
        {
            var mensagens = (new[] { mensagem }).Concat(outrasMensagens).ToList();
            var response = new VoidResponse();
            mensagens.ForEach(msg => response.AddErro(msg));
            return response;
        }

        public static Response<T> Erro<T>(string mensagem, params string[] outrasMensagens)
        {
            return Response.Erro<T>(default(T), mensagem, outrasMensagens);
        }

        public static Response<T> Erro<T>(T value, string mensagem, params string[] outrasMensagens)
        {
            var mensagens = (new[] { mensagem }).Concat(outrasMensagens).ToList();
            var response = new Response<T>(value);
            mensagens.ForEach(msg => response.AddErro(msg));
            return response;
        }

        public static VoidResponse Ok()
        {
            return new VoidResponse();
        }

        public static VoidResponse Ok(params string[] mensagens)
        {
            var response = new VoidResponse();
            Array.ForEach(mensagens, msg => response.AddSucesso(msg));
            return response;
        }

        public static Response<T> Ok<T>(T value, params string[] mensagens)
        {
            var response = new Response<T>(value);
            Array.ForEach(mensagens, msg => response.AddSucesso(msg));
            return response;
        }

        public static Response<T> Ok<T>(T value)
        {
            return new Response<T>(value);
        }
        
    }

    /// <summary>
    ///     Response com um value.
    /// </summary>
    public class Response<T> : VoidResponse, IResponse<T>
    {
        public T Value { get; set; }

        public bool HasValue
        {
            get
            {
                var typeOfT = typeof (T);

                // Se for tipo primitivo
                if (typeOfT.IsPrimitive)
                    return true;

                // Se for um reference type...
                if (!typeOfT.IsValueType)
                    return !Equals(Value, default(T));

                // Se for Nullable...
                if (typeOfT.IsGenericType && typeOfT.GetGenericTypeDefinition() == typeof (Nullable<>))
                    return !Equals(Value, default(T));

                // Se for Guid...
                if (typeOfT == typeof(Guid))
                    return !Equals(Value, default(T));

                else
                    return true;

            }
        }

        #region ' Constructor '

        public Response() : base() {  }

        public Response(T value) : this(value, null)
        {
        }

        public Response(T value, IEnumerable<Mensagem> mensagens) : base(mensagens)
        {
            Value = value;
        }

        public Response(T value, params Mensagem[] mensagens) : this(value, (IEnumerable<Mensagem>) mensagens)
        {
        }

        [Obsolete("Utilizar outro overloads")]
        //ToDo: Remover construtor obsoleto.
        public Response(params Mensagem[] mensagens) : base(mensagens)
        {
            if (mensagens == null) throw new ArgumentNullException("mensagens");
            if (mensagens.Length == 0) throw new ArgumentException("Nenhuma mensagem informada");
        }

        #endregion

        #region ' Operator '

        public static implicit operator Response<T>(T value)
        {
            return new Response<T>(value);
        }

        #endregion


    }

}
