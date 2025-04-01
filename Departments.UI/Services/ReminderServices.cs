using Departments.UI.Models;
using Departments.UI.Models.DTO;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace Departments.UI.Services
{

    public class ReminderServices : IReminderServices
    {
        private readonly ILogger<ReminderServices> logger;
        public readonly IHttpClientFactory HttpClientFactory;
        public ReminderServices(ILogger<ReminderServices> logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            HttpClientFactory = httpClientFactory;
        }
        public async Task<string> AddReminder(ReminderDto reminderDto)
        {
            var client = HttpClientFactory.CreateClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7138/api/Reminder/schedule"),
                Content = new StringContent(JsonSerializer.Serialize(reminderDto), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                logger.LogInformation("Finished add new Reminders, The Request from user view");
                return "Reminder Adding succeeded";
            }
            else
            {
                var errorContent = await httpResponseMessage.Content.ReadAsStringAsync();
                logger.LogError("Adding failed:" + errorContent);
                return errorContent;
            }
        }

        public async Task<List<ReminderModel>> GetAll()
        {
            var client = HttpClientFactory.CreateClient();
            var Response = await client.GetFromJsonAsync<List<ReminderModel>>("https://localhost:7138/api/Reminder");
            logger.LogInformation("Finished Get all reminders Request from user view");
            return Response?.ToList() ?? new List<ReminderModel>();
        }
        public async Task<string> RemoveReminder(Guid id)
        {
            var client = HttpClientFactory.CreateClient();
            var Response = await client.DeleteAsync($"https://localhost:7138/api/Reminder/{id}");
            Response.EnsureSuccessStatusCode();
            logger.LogInformation("Finished remove reminder Request from user view");
            return "OK";
        }
    }
}
