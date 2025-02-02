using UserSystem.Repositories.AuditLogRepository;

namespace UserSystem.Services.AuditLogService
{
    public class AuditLogService(IAuditLogRepository auditLogRepository) : IAuditLogService
    {
        public async Task AuditCreate(string entityName, string entityKey, string currentUserId)
        {
            await auditLogRepository.AuditCreateAsync(entityName, entityKey, currentUserId);
        }

        public async Task AuditUpdate(string entityName, string entityKey, string currentUserId, string oldValues, string newValues)
        {
            await auditLogRepository.AuditUpdateAsync(entityName, entityKey, currentUserId, oldValues, newValues);
        }

        public async Task AuditDelete(string entityName, string entityKey, string currentUserId)
        {
            await auditLogRepository.AuditDeleteAsync(entityName, entityKey, currentUserId);
        }
    }
}
