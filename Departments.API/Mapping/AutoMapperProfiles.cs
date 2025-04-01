using AutoMapper;
using Departments.API._Models.Domain;
using Departments.API._Models.DTO;

namespace Departments.API.Mapping
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<AddDepartmentRequestDto, Department>().ReverseMap();
            CreateMap<UpdateDepartmentRequestDto, Department>().ReverseMap();
            CreateMap<AddReminderRequestDto, Reminder>().ReverseMap();
           
        }
    }
}
