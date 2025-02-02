using Configurations.Interfaces;

namespace UserSystem.Repositories.AuditLogRepository
{
    public interface IAuditLogRepository : IScopedRepository
    {
        Task AddAuditLogAsync(string entityName, string action, string entityKey, string currentUserId, string oldValues = null, string newValues = null);
        Task AuditCreateAsync(string entityName, string entityKey, string currentUserId);
        Task AuditUpdateAsync(string entityName, string entityKey, string currentUserId, string oldValues, string newValues);
        Task AuditDeleteAsync(string entityName, string entityKey, string currentUserId);
    }
}
