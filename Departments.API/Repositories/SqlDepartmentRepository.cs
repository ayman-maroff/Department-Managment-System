using Departments.API._Models.Domain;
using Departments.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Departments.API.Repositories
{
    public class SqlDepartmentRepository : IDepartmentRepository

    {
        private readonly DeparmentsDbContext dbContext;
        public SqlDepartmentRepository(DeparmentsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Department> CreateAsync(Department department)
        {
            if (department.ParentId.HasValue)
            {
                // If there is a parent department, add the new department to the parent's SubDepartments collection
                var parentDepartment = await GetByIdAsync(department.ParentId.Value);

                if (parentDepartment != null)
                {
                    // Add the new department to the parent's SubDepartments collection
                    parentDepartment.SubDepartments.Add(department);

                    // Update the parent department in the database
                    dbContext.Update(parentDepartment);
                }
            }
            await dbContext.Departments.AddAsync(department);
            await dbContext.SaveChangesAsync();
            return department;
        }

        public async Task<Department?> DeleteAsync(Guid id)
        { var department = await dbContext.Departments
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                throw new Exception("Department not found");
            }

            // Load all sub-departments recursively
            await LoadSubDepartmentsAsync(department);

            // Recursively delete all sub-departments
            await DeleteSubDepartmentsAsync(department);

            // Remove the parent department
            dbContext.Departments.Remove(department);

            // Save changes to the database
            await dbContext.SaveChangesAsync();
            return department;
        }

        private async Task DeleteSubDepartmentsAsync(Department department)
        {
            // Get all sub-departments
            var subDepartments = department.SubDepartments.ToList();

            foreach (var subDepartment in subDepartments)
            {
                // Recursively delete sub-departments
                await DeleteSubDepartmentsAsync(subDepartment);

                // Remove the reference to the parent department
                subDepartment.ParentId = null;
                dbContext.Departments.Update(subDepartment);

                // Now remove the current sub-department
                dbContext.Departments.Remove(subDepartment);
            }
        }
        private async Task LoadSubDepartmentsAsync(Department department)
        {
            // Load sub-departments explicitly (lazy loading alternative)
            var subDepartments = await dbContext.Departments
                .Where(d => d.ParentId == department.Id)
                .ToListAsync();

            department.SubDepartments = subDepartments;

            // Recursively load sub-departments for each sub-department
            foreach (var subDepartment in subDepartments)
            {
                await LoadSubDepartmentsAsync(subDepartment);  // Recursive call
            }
        }


        public async Task<List<Department>> GetTopDepartmentAsync()
        {
            return await dbContext.Departments.Where(d => d.ParentId == null).ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(Guid id)
        {
            return await dbContext.Departments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Department>> GetSubDepartmentAsync(Guid parentId)
        {
            var subDepartments =  await dbContext.Departments
                                  .Where(d => d.Id == parentId) // Find the department with the specific ID
                                  .Select(d => d.SubDepartments) // Select only its sub-departments
                                  .FirstOrDefaultAsync();
            return subDepartments?.ToList() ?? new List<Department>();
        }

        public async Task<Department?> UpdateAsync(Guid id, Department department)
        {
            // Retrieve the existing department from the database
            var DomainDepartment = await GetByIdAsync(id);
            if (DomainDepartment == null)
            {
                return null;
            }

            // If the ParentId has changed, update the parent relationships
            if (department.ParentId != DomainDepartment.ParentId)
            {
                // Remove from the old parent's SubDepartments collection
                if (DomainDepartment.ParentId != null)
                {
                    var oldParentDepartment = await GetByIdAsync((Guid)DomainDepartment.ParentId);
                    if (oldParentDepartment != null)
                    {
                        oldParentDepartment.SubDepartments.Remove(DomainDepartment);
                    }
                }

                // Add to the new parent's SubDepartments collection
                if (department.ParentId != null)
                {
                    var newParentDepartment = await GetByIdAsync(department.ParentId.Value);
                    if (newParentDepartment != null)
                    {
                        newParentDepartment.SubDepartments.Add(DomainDepartment);
                    }
                }
            }

            // Update properties of the existing department
            DomainDepartment.Name = department.Name;
            DomainDepartment.Email = department.Email;
            DomainDepartment.DepartmentLogoUrl = department.DepartmentLogoUrl;
            DomainDepartment.ParentId = department.ParentId;

            // Save changes to the database
            await dbContext.SaveChangesAsync();
            return DomainDepartment;
        }

        public async Task<bool> IsDepartmentNameUniqueAsync(string name, Guid? id = null)
        {
            if(id == null)
            {
                return await dbContext.Departments
                                        .Where(d => d.Name == name)
                                        .AnyAsync() == false;
            }
            else
            {
                return await dbContext.Departments
                                  .Where(d => d.Name == name&& d.Id!=id)
                                  .AnyAsync() == false;
            }
         
        }

        public async Task<List<Department>> GetAllDepartmentAsync()
        {
            return await dbContext.Departments.ToListAsync();
        }

        public async Task<bool> IsDepartmentEmailUniqueAsync(string email, Guid? id = null)
        {
            if (id == null)
            {
                return await dbContext.Departments
                                        .Where(d => d.Email == email)
                                        .AnyAsync() == false;
            }
            else
            {
                return await dbContext.Departments
                                  .Where(d => d.Email == email && d.Id != id)
                                  .AnyAsync() == false;
            }
        }
    }
}
