using LeadManagement.Domain.Entities;

namespace LeadManagement.Domain.Repositories
{
    public interface ILeadRepository
    {
        Task<Lead> GetLeadByIdAsync(int id);
        Task<List<Lead>> GetLeadsByStatusAsync(string status);
        Task UpdateLeadAsync(Lead lead);
    }
}
