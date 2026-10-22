using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Common
{
    public class EntradaInvalidaException : Exception
    {
        public EntradaInvalidaException() {}
        public EntradaInvalidaException(string mensagem) : base(mensagem) {}
        public EntradaInvalidaException(string mensagem, Exception innerException) : base(mensagem, innerException) { }
    }
}
