using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace KGTT_Educate.Services.Account.Data.Repository
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        private AppDbContext _context { get; set; }
        public GroupRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Group group)
        {
            _context.Groups.Update(group);
        }
    }
}
