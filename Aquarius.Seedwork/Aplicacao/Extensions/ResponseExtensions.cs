using System;

namespace Vvs.Domain.Seedwork.Aplicacao.Extensions
{
    public static class ResponseExtensions
    {
        public static TResponse AddAlerta<TResponse>(this TResponse response, string texto) where TResponse : VoidResponse
        {
            if (response == null) throw new ArgumentNullException("response");
            if (texto == null) throw new ArgumentNullException("texto");

            response.AddMensagem(TipoMensagem.Alerta, texto);

            return response;
        }

        public static TResponse AddInfo<TResponse>(this TResponse response, string texto) where TResponse : VoidResponse
        {
            if (response == null) throw new ArgumentNullException("response");
            if (texto == null) throw new ArgumentNullException("texto");

            response.AddMensagem(TipoMensagem.Info, texto);
            return response;
        }

        public static TResponse AddSucesso<TResponse>(this TResponse response, string texto) where TResponse : VoidResponse
        {
            if (response == null) throw new ArgumentNullException("response");
            if (texto == null) throw new ArgumentNullException("texto");

            response.AddMensagem(TipoMensagem.Sucesso, texto);
            return response;

        }

        public static TResponse AddErro<TResponse>(this TResponse response, string texto) where TResponse : VoidResponse
        {
            if (response == null) throw new ArgumentNullException("response");
            if (texto == null) throw new ArgumentNullException("texto");

            response.AddMensagem(TipoMensagem.Erro, texto);
            return response;

        }

        public static TResponse AddErroCritico<TResponse>(this TResponse response, string texto) where TResponse : VoidResponse
        {
            if (response == null) throw new ArgumentNullException("response");
            if (texto == null) throw new ArgumentNullException("texto");

            response.AddMensagem(TipoMensagem.ErroCritico, texto);

            return response;
        }

    }
}
