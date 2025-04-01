using AutoMapper;
using AYWalks.API.CustomActionFilters;
using Departments.API._Models.Domain;
using Departments.API._Models.DTO;
using Departments.API.Data;
using Departments.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Departments.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {

        private readonly IDepartmentRepository departmentRepository;
        private readonly IMapper mapper;
        private readonly ILogger logger;
        public DepartmentsController(IDepartmentRepository departmentRepository,IMapper mapper, ILogger<DepartmentsController> logger)
        {

            this.departmentRepository = departmentRepository;
            this.mapper = mapper;
            this.logger = logger;
            
        }
        [HttpGet]
        public async Task<IActionResult> GetTopDepartment()
        {

            var departmentsDomain = await departmentRepository.GetTopDepartmentAsync();
            logger.LogInformation(" Finished GetAll Top Departments Request");
            //logger.LogInformation($" Finished GetAll Top Departments Request With data:{JsonSerializer.Serialize(departmentsDomain)}");
            return Ok(mapper.Map<List<DepartmentDto>>(departmentsDomain));
        }
        [HttpGet]
        [Route("all-departments")]
        public async Task<IActionResult> GetAllDepartment()
        {

            var departmentsDomain = await departmentRepository.GetAllDepartmentAsync();
            logger.LogInformation(" Finished GetAll Departments Request");
            //logger.LogInformation($" Finished GetAll Top Departments Request With data:{JsonSerializer.Serialize(departmentsDomain)}");
            return Ok(mapper.Map<List<DepartmentDto>>(departmentsDomain));
        }
        [HttpGet]
        [Route("{id:Guid}/sub-departments")]
        public async Task<IActionResult> GetSubDepartment([FromRoute] Guid id)
        {
            var departmentsDomain = await departmentRepository.GetSubDepartmentAsync(id);
            logger.LogInformation(" Finished GetSubDepartment Request");
            return Ok(mapper.Map<List<DepartmentDto>>(departmentsDomain));
        }
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
           
            var departmentsDomain = await departmentRepository.GetByIdAsync(id);
            logger.LogInformation(" Finished Get Department By id  Request");
            if (departmentsDomain == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<DepartmentDto>(departmentsDomain));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddDepartmentRequestDto addDepartmentRequestDto)
        {
            if (!await departmentRepository.IsDepartmentNameUniqueAsync(addDepartmentRequestDto.Name))
            {
                return BadRequest("This Department name already exists.");
            }
            if (!await departmentRepository.IsDepartmentEmailUniqueAsync(addDepartmentRequestDto.Email))
            {
                return BadRequest("This Email already exists.");
            }
            // map dto to domaian model;
            var departmentsDomainModel = mapper.Map<Department>(addDepartmentRequestDto);
            departmentsDomainModel = await departmentRepository.CreateAsync(departmentsDomainModel);
            logger.LogInformation(" Finished Create new Department Request");
            return CreatedAtAction(nameof(GetById), new { id = departmentsDomainModel.Id }, mapper.Map<DepartmentDto>(departmentsDomainModel));
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var departmentsDomainModel = await departmentRepository.DeleteAsync(id);
            logger.LogInformation(" Finished Delete Department Request ");
            if (departmentsDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<DepartmentDto>(departmentsDomainModel));
        }
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateDepartmentRequestDto updateDepartmentRequestDto)
        {
            if (!await departmentRepository.IsDepartmentNameUniqueAsync(updateDepartmentRequestDto.Name, id))
            {
                return BadRequest("This Department name already exists.");
            }
            if (!await departmentRepository.IsDepartmentEmailUniqueAsync(updateDepartmentRequestDto.Email, id))
            {
                return BadRequest("This Email already exists.");
            }
            var departmentsDomainModel = mapper.Map<Department>(updateDepartmentRequestDto);
            departmentsDomainModel = await departmentRepository.UpdateAsync(id, departmentsDomainModel);
            logger.LogInformation(" Finished Update Department Request");
            if (departmentsDomainModel == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<DepartmentDto>(departmentsDomainModel));
        }
    }
}
