using AutoMapper;
using AYWalks.API.CustomActionFilters;
using Departments.API._Models.Domain;
using Departments.API._Models.DTO;
using Departments.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Departments.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderRepository reminderRepository;
        private readonly IMapper mapper;
        private readonly ILogger logger;
        public ReminderController(IReminderRepository reminderRepository, IMapper mapper, ILogger<ReminderController> logger)
        {
            this.reminderRepository = reminderRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpPost("schedule")]
        [ValidateModel]
        public IActionResult ScheduleReminder([FromBody] AddReminderRequestDto addReminderRequestDto)
        {
            var reminderDomainModel = mapper.Map<Reminder>(addReminderRequestDto);

            reminderRepository.ScheduleEmail(reminderDomainModel);
            logger.LogInformation($" Finished add new Reminder to sent at :{JsonSerializer.Serialize(addReminderRequestDto.DateTimeToSend)}");
            return Ok("Reminder scheduled successfully.");
        }
        [HttpGet]
        public async Task<IActionResult> GetAllReminders()
        {

            var remindersDomain = await reminderRepository.GetAllRemindersAsync();
            logger.LogInformation(" Finished GetAll Reminders Request");
            return Ok(remindersDomain);
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> RemoveReminder([FromRoute] Guid id)
        {
            var Response = await reminderRepository.RemoveReminder(id);
            logger.LogInformation(" Finished remove Reminder Request ");
            if (Response == false)
            {
                return NotFound("Not Found");
            }
            return Ok("Reminder Remove Successfully");
        }
    }
}
