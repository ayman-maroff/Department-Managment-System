using System.ComponentModel.DataAnnotations;

namespace Departments.UI.Models.DTO
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Name has to be Maximum 100 characters")]
        public string Name { get; set; }

        public string? DepartmentLogoUrl { get; set; }

        public Guid? ParentId { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public LogoModel? FileUpload { get; set; }

    }
}
