using Configurations.Interfaces;

namespace UserSystem.Services.AuditLogService
{
    public interface IAuditLogService : IScopedService
    {
        Task AuditCreate(string entityName, string entityKey, string currentUserId);
        Task AuditUpdate(string entityName, string entityKey, string currentUserId, string oldValues, string newValues);
        Task AuditDelete(string entityName, string entityKey, string currentUserId);
    }
}
