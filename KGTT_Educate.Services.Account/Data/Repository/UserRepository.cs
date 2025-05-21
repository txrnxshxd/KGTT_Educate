using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;

namespace KGTT_Educate.Services.Account.Data.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }
    }
}
