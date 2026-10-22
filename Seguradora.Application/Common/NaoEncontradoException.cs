using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Common
{
    public class NaoEncontradoException : Exception
    {
        public NaoEncontradoException() { }
        public NaoEncontradoException(string message) : base(message) { }
        public NaoEncontradoException(string message, Exception innerException) : base(message, innerException) { }
    }
}
