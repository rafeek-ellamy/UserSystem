using UserSystem.Data;
using UserSystem.Data.Entities;

namespace UserSystem.Repositories.AuditLogRepository
{
    public class AuditLogRepository(UserSystemDbContext context) : IAuditLogRepository
    {
        public async Task AddAuditLogAsync(string entityName, string action, string entityKey, string currentUserId, string oldValues = null, string newValues = null)
        {
            var auditLog = new AuditLog
            {
                EntityName = entityName,
                Action = action,
                EntityKey = entityKey,
                CreateAt = DateTime.UtcNow,
                User = currentUserId,
                OldValues = oldValues,
                NewValues = newValues
            };

            await context.AuditLogs.AddAsync(auditLog);
            await context.SaveChangesAsync();
        }

        public async Task AuditCreateAsync(string entityName, string entityKey, string currentUserId)
        {
            await AddAuditLogAsync(entityName, "Create", entityKey, currentUserId);
        }

        public async Task AuditUpdateAsync(string entityName, string entityKey, string currentUserId, string oldValues, string newValues)
        {
            await AddAuditLogAsync(entityName, "Update", entityKey, currentUserId, oldValues, newValues);
        }

        public async Task AuditDeleteAsync(string entityName, string entityKey, string currentUserId)
        {
            await AddAuditLogAsync(entityName, "Delete", entityKey, currentUserId);
        }
    }
}
