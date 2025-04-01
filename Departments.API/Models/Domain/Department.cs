using System.ComponentModel.DataAnnotations;

namespace Departments.API._Models.Domain
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string? DepartmentLogoUrl { get; set; }

        public Guid? ParentId { get; set; }
        public Department? ParentDepartment { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public ICollection<Department> SubDepartments { get; set; } = new List<Department>();
    }
}
