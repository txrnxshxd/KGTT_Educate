using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;

namespace KGTT_Educate.Services.Account.Data.Repository
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        private AppDbContext _context;
        public RoleRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Role role)
        {
            _context.Roles.Add(role);
        }
    }
}
