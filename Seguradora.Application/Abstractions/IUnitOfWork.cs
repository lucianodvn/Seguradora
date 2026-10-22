using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Application.Abstractions
{
    public interface IUnitOfWork
    {
        Task<int> SalvarAsync(CancellationToken cancellationToken);
    }
}
