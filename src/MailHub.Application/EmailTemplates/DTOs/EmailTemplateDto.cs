namespace MailHub.Application.EmailTemplates.DTOs
{
    public class EmailTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Parameters { get; set; } = new();
    }
}
