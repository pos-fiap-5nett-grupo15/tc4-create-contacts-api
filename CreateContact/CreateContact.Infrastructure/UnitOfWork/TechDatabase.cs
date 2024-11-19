using CreateContact.Infrastructure.Crypto;
using Microsoft.Extensions.Configuration;

namespace CreateContact.Infrastructure.UnitOfWork
{
    public class TechDatabase : BaseConnection, ITechDatabase
    {
        public TechDatabase(IConfiguration configuration, ICryptoService cryptoService) : base(configuration, cryptoService)
        {

        }
    }
}
