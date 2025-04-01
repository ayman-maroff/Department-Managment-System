namespace Departments.UI.Models
{
    public class ReminderModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime DateTimeToSend { get; set; }
        public string SenderEmail { get; set; } // Email of the sender department
        public IList<string> RecipientsEmail { get; set; }
        public long? HangfireJobId { get; set; }
    }
}
