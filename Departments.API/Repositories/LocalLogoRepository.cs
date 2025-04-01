
using Departments.API._Models.Domain;
using Departments.API.Data;

namespace Departments.API.Repositories
{
    public class LocalLogoRepository : ILogoRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly DeparmentsDbContext dbContext;

        public LocalLogoRepository(IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            DeparmentsDbContext dbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }


        public async Task<Logo> Upload(Logo logo)
        {
            var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Logos",
                $"{logo.FileName}{logo.FileExtension}");

            // Upload logo to Local Path
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await logo.File.CopyToAsync(stream);

            // https://localhost:1234/logos/logo.jpg

            var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Logos/{logo.FileName}{logo.FileExtension}";

            logo.FilePath = urlFilePath;


            // Add Image to the Logos table
            await dbContext.Logos.AddAsync(logo);
            await dbContext.SaveChangesAsync();

            return logo;
        }
    }
}