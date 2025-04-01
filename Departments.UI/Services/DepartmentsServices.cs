using Departments.UI.Models;
using Departments.UI.Models.DTO;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;

namespace Departments.UI.Services
{
    public class DepartmentsServices : IDepartmentsServices
    {
        private readonly ILogger<DepartmentsServices> logger;
        public readonly IHttpClientFactory HttpClientFactory;
        public DepartmentsServices(ILogger<DepartmentsServices> logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            HttpClientFactory = httpClientFactory;
        }
        public async Task<string> CreateDepartment(DepartmentModel departmentModel)
        {
            var client = HttpClientFactory.CreateClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7138/api/Departments"),
                Content = new StringContent(JsonSerializer.Serialize(departmentModel), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                logger.LogInformation("Finished create new Departments, The Request from user view");
                return "Adding succeeded";
            }
            else
            {
                var errorContent = await httpResponseMessage.Content.ReadAsStringAsync();
                logger.LogError("Adding failed:" + errorContent);
                return errorContent;
            }
        }

        public async Task<string> DeleteDepartment(Guid id)
        {
            var client = HttpClientFactory.CreateClient();
            var Response = await client.DeleteAsync($"https://localhost:7138/api/Departments/{id}");
            Response.EnsureSuccessStatusCode();
            logger.LogInformation("Finished delete Departments Request from user view");
            return await Response.Content.ReadAsStringAsync();
        }

        public async Task<List<DepartmentDto>> GetAllDepartment()
        {
            var client = HttpClientFactory.CreateClient();
            var Response = await client.GetFromJsonAsync<List<DepartmentDto>>("https://localhost:7138/api/Departments/all-departments");
            logger.LogInformation("Finished Get all Departments Request from user view");
            return Response?.ToList() ?? new List<DepartmentDto>();
        }

        public async Task<DepartmentDto> GetDepartmentById(Guid id)
        {
            var client = HttpClientFactory.CreateClient();
            var Response = await client.GetFromJsonAsync<DepartmentDto>($"https://localhost:7138/api/Departments/{id.ToString()}");
            logger.LogInformation("Finished Get  Departments by id Request from user view");
            return Response ?? new DepartmentDto();
        }

        public async Task<List<DepartmentDto>> GetSubDepartment(Guid id)
        {
            var client = HttpClientFactory.CreateClient();
            var httpResponseMessage = await client.GetAsync($"https://localhost:7138/api/Departments/{id}/sub-departments");
            httpResponseMessage.EnsureSuccessStatusCode();
            logger.LogInformation("Finished Get Sub Departments Request from user view");
            var subDepartments = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<DepartmentDto>>();
            // Convert IEnumerable to List
            return subDepartments?.ToList() ?? new List<DepartmentDto>();
        }


        public async Task<List<DepartmentDto>> GetTopDepartmentAsync()
        {
            List<DepartmentDto> response = new List<DepartmentDto>();
            //get all top Departments
            var client = HttpClientFactory.CreateClient();
            var httpResponseMessage = await client.GetAsync("https://localhost:7138/api/Departments");
            httpResponseMessage.EnsureSuccessStatusCode();
            logger.LogInformation(" Finished GetAll Top Departments Request from user view");
            //convert json to object
            response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<DepartmentDto>>());
            return response;
        }

        public List<DepartmentDto> RemoveSubDepartments(Guid parentId, List<DepartmentDto> departments)
        {
            // Filter out departments whose ParentId is the current parentId
            var filteredDepartments = departments.Where(d => d.ParentId != parentId).ToList();
            // Now check if any department left in the list has the current parentId as its ParentId,
            // and remove its sub-departments as well
            foreach (var department in departments.Where(d => d.ParentId == parentId).ToList())
            {
                filteredDepartments = RemoveSubDepartments(department.Id, filteredDepartments);
            }
            return filteredDepartments;
        }

        public async Task<string> UploadDepartmentLogo(LogoModel logoModel)
        {
            var client = HttpClientFactory.CreateClient();
            // Create the multipart form data content to send the file
            var multipartContent = new MultipartFormDataContent();
            // Add the file stream to the form data
            var fileContent = new StreamContent(logoModel.File.OpenReadStream());
            fileContent.Headers.Add("Content-Type", logoModel.File.ContentType);  // Ensure content type is correctly set
            multipartContent.Add(fileContent, "file", logoModel.FileName);  // "file" must match the expected field name in the API
            // Add the file name and description (fileDescription is optional here)
            multipartContent.Add(new StringContent(logoModel.FileName ?? ""), "fileName");  // Add fileName
            multipartContent.Add(new StringContent(logoModel.FileDescription ?? ""), "fileDescription");  // Add fileDescription
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7138/api/Logos/Upload"),
                Content = multipartContent
            };
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            // Ensure successful response
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                logger.LogInformation("Finished Upload new Department logo, The Request from user view");
                return "Upload succeeded";
            }

            else
            {
                var errorContent = await httpResponseMessage.Content.ReadAsStringAsync();
                logger.LogError("Uplaod failed:" + errorContent);
                return errorContent;
            }
        }

        public async Task<List<DepartmentDto>> RemoveParentSubDepartments(Guid parentId)
        {
            var response = await GetAllDepartment();
            if (response != null)
            {
                response = response.Where(d => d.Id != parentId).ToList();
                return RemoveSubDepartments(parentId, response);
            }
            else return new List<DepartmentDto>();
        }

        public async Task<string> EditDepartment(DepartmentModel departmentModel, Guid id)
        {
            var client = HttpClientFactory.CreateClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7138/api/Departments/{id.ToString()}"),
                Content = new StringContent(JsonSerializer.Serialize(departmentModel), Encoding.UTF8, "application/json")
            };
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                logger.LogInformation("Finished Edit Department, The Request from user view");
                return "Editing succeeded";
            }
            else
            {
                var errorContent = await httpResponseMessage.Content.ReadAsStringAsync();
                logger.LogError("Editing failed:" + errorContent);
                return errorContent;
            }
        }
        public async Task<List<DepartmentDto>> GetAllSubDepartments(Guid id)
        {
            var allSubDepartments = new List<DepartmentDto>();

            // Get direct sub-departments
            var subDepartments = await GetSubDepartment(id);

            // Add direct sub-departments to the list
            allSubDepartments.AddRange(subDepartments);

            // Recursively get sub-departments for each sub-department
            foreach (var subDept in subDepartments)
            {
                var subSubDepartments = await GetAllSubDepartments(subDept.Id);
                allSubDepartments.AddRange(subSubDepartments);
            }

            logger.LogInformation("Finished getting all sub-departments recursively for Department Id: {id}", id);

            return allSubDepartments;
        }
        public async Task<List<DepartmentDto>> GetAllParentDepartments(Guid id)
        {
            var allParentDepartments = new List<DepartmentDto>();

            // Get the current department by its ID
            var currentDepartment = await GetDepartmentById(id);

            // If the current department has a parent
            while (currentDepartment.ParentId.HasValue)
            {
                // Get the parent department
                var parentDepartment = await GetDepartmentById(currentDepartment.ParentId.Value);

                // Add the parent to the list
                allParentDepartments.Add(parentDepartment);

                // Move to the next parent
                currentDepartment = parentDepartment;
            }

            logger.LogInformation("Finished getting all parent departments recursively for Department Id: {id}", id);

            return allParentDepartments;
        }


    }
}
