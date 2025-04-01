using Departments.API._Models.Domain;

namespace Departments.API.Repositories
{
    public interface IReminderRepository
    {
        Task<Reminder> ScheduleEmail(Reminder reminder);
        Task<List<Reminder>> GetAllRemindersAsync();
        Task<bool> RemoveReminder(Guid reminderId);
    }
}
