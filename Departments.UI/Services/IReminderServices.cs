using Departments.UI.Models;
using Departments.UI.Models.DTO;

namespace Departments.UI.Services
{
    public interface IReminderServices
    {
        Task<string> AddReminder(ReminderDto reminderDto);
        Task<List<ReminderModel>> GetAll();
        Task<string> RemoveReminder(Guid id);
    }
}
