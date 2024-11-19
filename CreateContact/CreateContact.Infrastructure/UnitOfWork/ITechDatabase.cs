using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateContact.Infrastructure.UnitOfWork
{
    public interface ITechDatabase : IDisposable
    {
        IDbConnection Connection { get; }

        void EnsureConnectionIdOpen();
    }
}
