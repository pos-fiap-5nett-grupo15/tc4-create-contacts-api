using CreateContact.Infrastructure.Repositories.Contact;
using TechChallenge.Infrastructure.UnitOfWork;

namespace CreateContact.Infrastructure.UnitOfWork
{
    public sealed class CreateContactUnitOfWork : BaseUnitOfWork, ICreateContactUnitOfWork
    {
        private readonly ITechDatabase _techDabase;

        public IContactRepository ContactRepository { get; }

        public CreateContactUnitOfWork(ITechDatabase database)
            : base(database)
        {
            this._techDabase = database ?? throw new ArgumentNullException(nameof(database));

            this.ContactRepository = new ContactRepository(this._techDabase);
        }
    }
}
