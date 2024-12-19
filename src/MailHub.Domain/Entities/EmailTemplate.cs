using MailHub.Domain.Common;

namespace MailHub.Domain.Entities
{
    public class EmailTemplate : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Parameters { get; set; } = new();
        public bool IsDeleted { get; set; } = false;  // Soft delete flag
    }
}
