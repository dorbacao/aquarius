using System.Collections.Generic;

namespace Vvs.Domain.Seedwork.Aplicacao
{
    public interface IResponse
    {
        IEnumerable<Mensagem> Mensagens { get; }
    }

    public interface IResponse<out T> : IResponse
    {
        T Value { get; }
        bool HasValue { get; }

    }
}