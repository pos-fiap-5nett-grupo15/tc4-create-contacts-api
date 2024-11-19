using CreateContact.Infrastructure.Repositories.Contact;
using System.Transactions;

namespace CreateContact.Infrastructure.UnitOfWork
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ITechDatabase _techDabase;

        public IContactRepository ContactRepository { get; }

        public UnitOfWork(ITechDatabase database)
        {
            this._techDabase = database;

            this.ContactRepository = new ContactRepository(this._techDabase);
        }

        #region Transaction methods:
        public ITransaction BeginTransaction()
        {
            var tx = new Transaction();
            this._techDabase.EnsureConnectionIdOpen();
            return tx;
        }

        public ITransaction BeginTransaction(TransactionOptions transactionOptions)
        {
            var tx = new Transaction(transactionOptions);
            this._techDabase.EnsureConnectionIdOpen();
            return tx;
        }
        #endregion Transaction methods.

        #region IDispose Support
        public bool disposidedValue = false;

        void Dispose(bool disposing)
        {
            if (disposidedValue)
            {
                this._techDabase.Dispose();
            }
            disposidedValue = true;
        }


        public void Dispose() => this.Dispose(true);

        #endregion


    }
}
