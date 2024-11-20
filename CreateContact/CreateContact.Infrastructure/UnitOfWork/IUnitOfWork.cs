using CreateContact.Infrastructure.Repositories.Contact;
using System.Transactions;
using TechChallenge.Infrastructure.UnitOfWork;

namespace CreateContact.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IContactRepository ContactRepository { get; }

        ITransaction BeginTransaction();
        ITransaction BeginTransaction(TransactionOptions transactionOptions);
    }
}
