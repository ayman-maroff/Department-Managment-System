namespace Departments.UI.Models.DTO
{
    public class DepartmentHierarchyViewDto
    {
        public Guid RootDepartmentId { get; set; }
        public List<DepartmentDto> ParentDepartments { get; set; }
        public List<DepartmentDto> SubDepartments { get; set; }
    }
}
