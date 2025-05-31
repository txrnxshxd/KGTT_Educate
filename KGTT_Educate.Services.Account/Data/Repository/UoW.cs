using KGTT_Educate.Services.Account.Data.Repository.Interfaces;

namespace KGTT_Educate.Services.Account.Data.Repository
{
    public class UoW : IUoW
    {
        private AppDbContext _context;

        public IGroupRepository Groups { get; private set; }

        public IRoleRepository Roles { get; private set; }

        public IUserGroupRepository UserGroup { get; private set; }

        public IUserRepository Users { get; private set; }

        public IUserRoleRepository UserRole { get; private set; }

        public UoW(AppDbContext context)
        {
            _context = context;
            Groups = new GroupRepository(context);
            Roles = new RoleRepository(context);
            UserGroup = new UserGroupRepository(context);
            Users = new UserRepository(context);
            UserRole = new UserRoleRepository(context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
