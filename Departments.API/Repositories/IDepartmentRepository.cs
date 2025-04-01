using Departments.API._Models.Domain;

namespace Departments.API.Repositories
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetTopDepartmentAsync();
        Task<List<Department>> GetAllDepartmentAsync();
        Task<List<Department>> GetSubDepartmentAsync(Guid parentId);
        Task<Department?> GetByIdAsync(Guid id);
        Task<Department> CreateAsync(Department department);
        Task<Department?> UpdateAsync(Guid id, Department department);
        Task<Department?> DeleteAsync(Guid id);
        Task<bool> IsDepartmentNameUniqueAsync(string name , Guid? id = null);
        Task<bool> IsDepartmentEmailUniqueAsync(string email, Guid? id = null);
    }
}
