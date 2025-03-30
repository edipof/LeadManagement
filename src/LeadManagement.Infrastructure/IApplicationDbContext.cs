using LeadManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeadManagement.Infrastructure
{
    public interface IApplicationDbContext
    {
        DbSet<Lead> Leads { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
