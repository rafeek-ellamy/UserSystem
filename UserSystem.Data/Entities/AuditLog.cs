namespace UserSystem.Data.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public string Action { get; set; }
        public string EntityKey { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime CreateAt { get; set; }
        public string User { get; set; }
    }
}
