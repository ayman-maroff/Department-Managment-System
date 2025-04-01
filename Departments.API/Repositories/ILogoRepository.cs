

using Departments.API._Models.Domain;

namespace Departments.API.Repositories
{
    public interface ILogoRepository
    {
        Task<Logo> Upload(Logo logo);
    }
}