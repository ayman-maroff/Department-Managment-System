using Departments.UI.Models.DTO;
using Departments.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Departments.UI.Controllers
{
    public class ReminderController : Controller
    {
        public readonly IDepartmentsServices departmentsServices;
        public readonly IReminderServices reminderServices;
        public ReminderController(IDepartmentsServices departmentsServices, IReminderServices reminderServices)
        {
            this.departmentsServices = departmentsServices;
            this.reminderServices = reminderServices;
        }
        [HttpGet]
        public async Task<IActionResult> Index(Guid id, Guid pid)
        {
            // Fetch sub-departments for the given department ID
            var subDepartments = await departmentsServices.GetSubDepartment(id) ?? new List<DepartmentDto>();
            ViewBag.SubDepartments = subDepartments;
            ViewBag.SenderId = id;
            // Check if the provided pid is empty (default Guid)
            if (pid == Guid.Empty)
            {
                // Fetch top departments if pid is not provided
                var topDepartments = await departmentsServices.GetTopDepartmentAsync() ?? new List<DepartmentDto>();
                topDepartments.RemoveAll(department => department.Id == id);
                ViewBag.TopDepartments = topDepartments;
                ViewBag.ParentDepartment = null;
            }
            else
            {
                // Fetch the parent department based on the provided pid
                var parentDepartment = await departmentsServices.GetDepartmentById(pid);
                ViewBag.ParentDepartment = parentDepartment;
                ViewBag.TopDepartments = null; // Optionally, you could omit this
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendReminder(List<string> SelectedDepartmentEmails, string description, DateTime time, Guid senderId)
        {
            var senderDepartment = await departmentsServices.GetDepartmentById(senderId);
            var reminder = new ReminderDto
            {
                Title = "Reminder Message from Department" + senderDepartment.Name,
                Body = description,
                DateTimeToSend = time,
                SenderEmail = senderDepartment.Email,
                RecipientsEmail = SelectedDepartmentEmails
            };
            var response = await reminderServices.AddReminder(reminder);
            if (response == "Reminder Adding succeeded")
            {
                TempData["SuccessMessage"] = "The reminder has been added successfully, it will be sent in:" + time;
                return RedirectToAction("Index", "Departments");
            }
            else
            {
                ModelState.AddModelError("AddReminder", $"Error: {response}");
                return View();
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await reminderServices.GetAll();
            return View(response);
        }
        [HttpGet]
        public async Task<IActionResult> Remove(Guid id)
        {
            var response = await reminderServices.RemoveReminder(id);
            if (response == "OK")
            {
                TempData["SuccessMessage"] = "Reminder Removed successfully!";
                return RedirectToAction("GetAll", "Reminder");
            }
            else
            {
                return RedirectToAction("Index", "Reminder");
            }
   
        }
    }
}
