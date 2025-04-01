using Departments.API._Models.Domain;
using Departments.API._Models.DTO;
using Departments.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



namespace Departments.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogosController : ControllerBase
    {
        private readonly ILogoRepository logoRepository;

        public LogosController(ILogoRepository logoRepository)
        {
            this.logoRepository = logoRepository;
        }


        // POST: /api/Logos/Upload
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] LogoUploadRequestDto request)
        {
            ValidateFileUpload(request);

            if (ModelState.IsValid)
            {
                // convert DTO to Domain model
                var fileExtension = Path.GetExtension(request.File.FileName).ToLower();
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(request.FileName);

                // Ensure the FileName does not have any extension and append the correct extension
                var cleanFileName = fileNameWithoutExtension;

                var logoDomainModel = new Logo
                {
                    File = request.File,
                    FileExtension = fileExtension,
                    FileSizeInBytes = request.File.Length,
                    FileName = cleanFileName,  // Use the cleaned file name
                    FileDescription = request.FileDescription,
                };


                // User repository to upload logo
                await logoRepository.Upload(logoDomainModel);

                return Ok(logoDomainModel);

            }

            return BadRequest(ModelState);
        }


        private void ValidateFileUpload(LogoUploadRequestDto request)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension");
            }

            if (request.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size more than 10MB, please upload a smaller size file.");
            }
        }
    }
}