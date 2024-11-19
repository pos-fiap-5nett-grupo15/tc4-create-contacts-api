using CreateContact.Infrastructure.Repositories.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CreateContact.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IContactRepository ContactRepository { get; }

        ITransaction BeginTransaction();
        ITransaction BeginTransaction(TransactionOptions transactionOptions);
    }
}
