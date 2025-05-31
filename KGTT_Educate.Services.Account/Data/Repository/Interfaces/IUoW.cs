namespace KGTT_Educate.Services.Account.Data.Repository.Interfaces
{
    public interface IUoW
    {
        IGroupRepository Groups { get; }
        IRoleRepository Roles { get; }
        IUserGroupRepository UserGroup { get; }
        IUserRepository Users { get; }
        IUserRoleRepository UserRole { get; }

        void Save();
        Task SaveAsync();
    }
}
