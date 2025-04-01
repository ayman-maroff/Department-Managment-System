using Departments.API._Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace Departments.API._Models.DTO
{
    public class UpdateDepartmentRequestDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Name has to be Maximum 100 characters")]
        public string Name { get; set; }

        public string? DepartmentLogoUrl { get; set; }

        public Guid? ParentId { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
