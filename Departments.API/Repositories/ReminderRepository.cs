using Departments.API._Models.Domain;
using Departments.API.Data;
using Departments.API.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Departments.API.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IEmailService emailService;
        private readonly DeparmentsDbContext dbContext;
        private readonly ILogger logger;
        public ReminderRepository(IBackgroundJobClient backgroundJobClient, IEmailService emailService, DeparmentsDbContext dbContext , ILogger<ReminderRepository> logger)
        {
            this.backgroundJobClient = backgroundJobClient;
            this.emailService = emailService;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<List<Reminder>> GetAllRemindersAsync()
        {
            return await dbContext.Reminders.ToListAsync();
        }

        public async Task<Reminder> ScheduleEmail(Reminder reminder)
        {
            string jobIdString = backgroundJobClient.Schedule(() =>
                emailService.SendEmailAsync(
                    reminder.RecipientsEmail,
                    reminder.Title,
                    reminder.Body),
                reminder.DateTimeToSend);
            if (long.TryParse(jobIdString, out long jobId))
            {
                reminder.HangfireJobId = jobId;  // Store it as long in the reminder
            }
            else
            {
                logger.LogWarning("Failed to parse Hangfire job ID");
            }
            await dbContext.Reminders.AddAsync(reminder);
            await dbContext.SaveChangesAsync();
            return reminder;
        }
        public async Task<bool> RemoveReminder(Guid reminderId)
        {
                // Retrieve the reminder from the database by its ID
                var reminder = await dbContext.Reminders.FindAsync(reminderId);

                if (reminder == null)
                {
                    logger.LogWarning($"Reminder with ID {reminderId} not found.");
                    return false; // Reminder not found
                }

                // If the reminder has a Hangfire job scheduled, delete it using the HangfireJobId
                if (reminder.HangfireJobId.HasValue)
                {
                    bool deleted = BackgroundJob.Delete(reminder.HangfireJobId.ToString()); // Convert long to string for Hangfire
                    if (deleted)
                    {
                        logger.LogInformation($"Successfully removed scheduled reminder with HangfireJobId: {reminder.HangfireJobId}");
                    }
                    else
                    {
                        logger.LogWarning($"Failed to remove Hangfire job with ID {reminder.HangfireJobId}");
                        return false; // Job not found or already executed
                    }
                }
                // Remove the reminder from the database
                dbContext.Reminders.Remove(reminder);
                await dbContext.SaveChangesAsync();
                return true; // Reminder removed successfully
      
        }

    }
}
