namespace MailHub.ApiClient.Model
{
    public class EmailQueue
    {
        public int? TemplateId { get; set; } // Optional TemplateId
        public string TemplateName { get; set; } // Optional TemplateName
        public string Recipient { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
