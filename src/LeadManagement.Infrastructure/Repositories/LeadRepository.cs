using LeadManagement.Domain.Entities;
using LeadManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LeadManagement.Infrastructure.Repositories
{
    public class LeadRepository : ILeadRepository
    {
        private readonly IApplicationDbContext _context;

        public LeadRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Lead> GetLeadByIdAsync(int id)
        {
            return await _context.Leads.FirstOrDefaultAsync(lead => lead.Id == id);
        }

        public async Task<List<Lead>> GetLeadsByStatusAsync(string status)
        {
            var leads = await _context.Leads.Where(lead => lead.Status == status).ToListAsync();
            return leads;
        }

        public async Task UpdateLeadAsync(Lead lead)
        {
            lead.GetType().GetProperty("DateUpdated")?.SetValue(lead, DateTime.UtcNow);
            _context.Leads.Update(lead);
            await _context.SaveChangesAsync();
        }
    }
}
