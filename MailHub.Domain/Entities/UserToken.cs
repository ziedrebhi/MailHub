namespace MailHub.Domain.Entities
{

    public class UserToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiryDate { get; set; }

        public User User { get; set; }
    }
}
