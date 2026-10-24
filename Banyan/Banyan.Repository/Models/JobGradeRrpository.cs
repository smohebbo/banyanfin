
using Banyan.Entities.Models;
using Microsoft.EntityFrameworkCore;


namespace Banyan.Repository.Models
{
    public interface IAddressBookRepository : IRepositoryBase<AddressBook>
    {

    }
    public class AddressBookRepository : RepositoryBase<AddressBook>, IAddressBookRepository
    {
        private readonly DbContext _context;

        public AddressBookRepository(DbContext context) : base(context)
        {
            this._context = context;
        }

    }
}