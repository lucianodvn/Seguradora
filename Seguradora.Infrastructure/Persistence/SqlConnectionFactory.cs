using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguradora.Infrastructure.Persistence
{
    internal sealed record SqlConnectionFactory(string connectionString)
    {
        public SqlConnection Criar() => new(connectionString);
    }
}
