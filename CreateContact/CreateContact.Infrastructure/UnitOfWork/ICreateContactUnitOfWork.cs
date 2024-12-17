using CreateContact.Infrastructure.Repositories.Contact;
using System.Transactions;
using TechChallenge3.Infrastructure.UnitOfWork;

namespace CreateContact.Infrastructure.UnitOfWork
{
    public interface ICreateContactUnitOfWork : IBaseUnitOfWork
    {
        IContactRepository ContactRepository { get; }
    }
}
