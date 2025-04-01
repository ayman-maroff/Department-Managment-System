using Departments.UI.Models;
using Departments.UI.Models.DTO;

namespace Departments.UI.Services
{
    public interface IDepartmentsServices
    {
        Task<List<DepartmentDto>> GetAllSubDepartments(Guid id);
        Task<List<DepartmentDto>> GetAllParentDepartments(Guid id);
        Task<List<DepartmentDto>> GetTopDepartmentAsync();
        Task<List<DepartmentDto>> GetSubDepartment(Guid id);

        Task<List<DepartmentDto>> GetAllDepartment();
        Task<string> CreateDepartment(DepartmentModel departmentModel);
        Task<string> EditDepartment(DepartmentModel departmentModel,Guid id);
        Task<string> UploadDepartmentLogo(LogoModel logoModel);

        Task<string> DeleteDepartment(Guid id);

        Task<DepartmentDto> GetDepartmentById(Guid id);
        List<DepartmentDto> RemoveSubDepartments(Guid parentId, List<DepartmentDto> departments);
        Task<List<DepartmentDto>> RemoveParentSubDepartments(Guid parentId);

    }
}
