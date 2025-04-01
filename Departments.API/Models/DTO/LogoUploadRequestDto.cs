using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Departments.API._Models.DTO
{
    public class LogoUploadRequestDto
    {
        [NotMapped]
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public string FileName { get; set; }

        public string? FileDescription { get; set; }

    }
}
