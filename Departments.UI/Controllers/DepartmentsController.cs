using Departments.UI.Models;
using Departments.UI.Models.DTO;
using Departments.UI.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Departments.UI.Controllers
{
    public class DepartmentsController : Controller
    {
        public readonly IDepartmentsServices departmentsServices;
        public DepartmentsController(IDepartmentsServices departmentsServices)
        {
           this.departmentsServices = departmentsServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await departmentsServices.GetTopDepartmentAsync();
            return View(response);
        }
        [HttpGet]
        public async Task<IActionResult> DepartmentHierarchy(Guid id)
        {
            // Call the previously created functions
            var parentDepartments = await departmentsServices.GetAllParentDepartments(id);
            var subDepartments = await departmentsServices.GetAllSubDepartments(id);

            // Ensure parents are ordered from oldest to newest (root to leaf)
            parentDepartments.Reverse();  // Since we get parents starting from current, reverse them

            // Pass data to the view
            var model = new DepartmentHierarchyViewDto
            {
                RootDepartmentId =id,
                ParentDepartments = parentDepartments,
                SubDepartments = subDepartments
            };

            return View(model);
        }

        [HttpGet]
        [Route("Departments/GetSubDepartment/{id}")]
        public async Task<IActionResult> GetSubDepartment(Guid id)
        {
            var subDepartments = await departmentsServices.GetSubDepartment(id);
            return PartialView("_SubDepartmentsPartial", subDepartments);
        }

        [HttpGet]
        public IActionResult Create(Guid Pid, string Pname)
        {
            ViewData["ParentDepartmentId"] = Pid;
            ViewData["ParentDepartmentName"] = Pname;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(DepartmentDto model, Guid Pid, string Pname)
        {
            ViewData["ParentDepartmentId"] = Pid;
            ViewData["ParentDepartmentName"] = Pname;
            var newDepartment = new DepartmentModel
            {
                Name = model.Name,
                Email = model.Email,
                ParentId = model.ParentId == Guid.Empty ? null : model.ParentId,
                DepartmentLogoUrl = model.FileUpload?.FileName != null ? $"https://localhost:7138/Logos/{model.FileUpload.FileName}" : null
            };
            var response1 = await departmentsServices.CreateDepartment(newDepartment);

            if (response1== "Adding succeeded")
            {
                if (model.FileUpload != null && model.FileUpload.FileName != null)
                {
                    var logoModel = new LogoModel
                    {
                        File = model.FileUpload.File,
                        FileName = model.FileUpload.FileName,
                        FileDescription = null  // Set file description to null or leave it empty
                    };
                    var response2 = await departmentsServices.UploadDepartmentLogo(logoModel);
                    if (response2== "Upload succeeded")
                    {
                        TempData["SuccessMessage"] = "Department created successfully!";
                        return RedirectToAction("Index", "Departments");
                    }
                    else
                    {
                        ModelState.AddModelError("fileUpload", $"Error: {response2}");
                        return View();
                    }
                }
                else
                {
                    TempData["SuccessMessage"] = "Department created successfully!";
                    return RedirectToAction("Index", "Departments");
                }
            }
            ModelState.AddModelError("CreateFailed", $"Error: {response1}");
             return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await departmentsServices.DeleteDepartment(id);
            TempData["SuccessMessage"] = "Department deleted successfully!";
            return RedirectToAction("Index", "Departments");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Did)
        {
            var Response = await departmentsServices.GetDepartmentById(Did);
            if (Response != null)
            {
                //remove all subs department is under the current department we'II edit it
                //this action for edit department's parent so he cannot be child of his children
                var Response2 = await departmentsServices.RemoveParentSubDepartments(Did);
                if (Response2 != null)
                {
                    ViewBag.AllDepartmentExceptitChild = Response2;
                    ViewBag.Id = Did;
                    return View(Response);
                }
                ViewBag.AllDepartmentExceptitChild = null;
                return View(Response);
            }
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentDto model, Guid Id)
        {
            var Response = await departmentsServices.RemoveParentSubDepartments(Id);
            var CurrentDepartment = await departmentsServices.GetDepartmentById(Id);
            if (Response != null)
            {
                ViewBag.AllDepartmentExceptitChild = Response;
                ViewBag.Id = Id;
              
            }

            var departmentForModification = new DepartmentModel
            {
                Name = model.Name,
                Email = model.Email,
                ParentId = model.ParentId == Guid.Empty ? null : model.ParentId,
                DepartmentLogoUrl = model.FileUpload?.FileName != null ? $"https://localhost:7138/Logos/{model.FileUpload.FileName}" : model.DepartmentLogoUrl,
            };
            var response1 = await departmentsServices.EditDepartment(departmentForModification, Id);
            if (response1 == "Editing succeeded")
            {
                if (model.FileUpload != null && model.FileUpload.FileName != null)
                {
                    var logoModel = new LogoModel
                    {
                        File = model.FileUpload.File,
                        FileName = model.FileUpload.FileName,
                        FileDescription = null  // Set file description to null or leave it empty
                    };
                    var response2 = await departmentsServices.UploadDepartmentLogo(logoModel);
                    // Ensure successful response
                    if (response2== "Upload succeeded")
                    {
                        TempData["SuccessMessage"] = "Department updated successfully!";
                        return RedirectToAction("Index", "Departments");
                    }
                    else
                    {
                        ModelState.AddModelError("fileUpload", $"Error: {response2}");
                        return View(CurrentDepartment);
                    }
                }
                else
                {
                    TempData["SuccessMessage"] = "Department updated successfully!";
                    return RedirectToAction("Index", "Departments");
                }
            }
            else
            {
                ModelState.AddModelError("UpdateFailed", $"Error: {response1}");
                return View(CurrentDepartment);
            }
          
        }
        [HttpGet]
        public IActionResult Error(string message)
        {
            // Pass the error message to the view
            TempData["ErrorMessage"] = message;
            return RedirectToAction("Index", "Departments");
           
        }
    }
}
