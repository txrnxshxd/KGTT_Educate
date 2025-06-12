using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;
using Microsoft.EntityFrameworkCore;

namespace KGTT_Educate.Services.Account.Data.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Login == userName);

            if (user == null)
            {
                throw new Exception($"Пользователя с логином {userName} не существует");
            }

            return user;
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
