using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

namespace Baselib.Business.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IRepository<Department> _departments;
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IRepository<Department> departments, IUnitOfWork unitOfWork)
    {
        _departments = departments;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        var departments = await _departments.Query()
            .Include(d => d.ParentDepartment)
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();

        return departments.Select(MapToDto);
    }

    public async Task<IEnumerable<DepartmentDto>> GetTreeAsync()
    {
        var departments = await _departments.Query()
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();

        return BuildTree(departments, null);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var department = await _departments.Query()
            .Include(d => d.ParentDepartment)
            .Include(d => d.SubDepartments)
            .FirstOrDefaultAsync(d => d.Id == id);

        return department == null ? null : MapToDto(department);
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
    {
        await EnsureCodeIsUniqueAsync(dto.Code);

        var department = new Department
        {
            Name = dto.Name.Trim(),
            Code = dto.Code.Trim(),
            ParentDepartmentId = dto.ParentDepartmentId,
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        await _departments.AddAsync(department);
        await _unitOfWork.SaveChangesAsync();

        return (await GetByIdAsync(department.Id))!;
    }

    public async Task UpdateAsync(int id, UpdateDepartmentDto dto)
    {
        var department = await _departments.GetByIdAsync(id);
        if (department == null)
            throw new KeyNotFoundException("Department not found");

        if (dto.ParentDepartmentId == id)
            throw new InvalidOperationException("Department cannot be its own parent");

        await EnsureCodeIsUniqueAsync(dto.Code, id);

        department.Name = dto.Name.Trim();
        department.Code = dto.Code.Trim();
        department.ParentDepartmentId = dto.ParentDepartmentId;
        department.IsActive = dto.IsActive;
        department.UpdatedDate = DateTime.Now;

        await _departments.UpdateAsync(department);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var department = await _departments.GetByIdAsync(id);
        if (department == null)
            throw new KeyNotFoundException("Department not found");

        department.IsActive = false;
        department.UpdatedDate = DateTime.Now;
        await _departments.UpdateAsync(department);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task EnsureCodeIsUniqueAsync(string code, int? departmentId = null)
    {
        code = code.Trim();

        if (await _departments.AnyAsync(d => d.Code == code && (!departmentId.HasValue || d.Id != departmentId.Value)))
            throw new InvalidOperationException("Department code already exists");
    }

    private static List<DepartmentDto> BuildTree(IEnumerable<Department> departments, int? parentId)
    {
        return departments
            .Where(d => d.ParentDepartmentId == parentId)
            .Select(d =>
            {
                var dto = MapToDto(d);
                dto.SubDepartments = BuildTree(departments, d.Id);
                return dto;
            })
            .ToList();
    }

    private static DepartmentDto MapToDto(Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            ParentDepartmentId = department.ParentDepartmentId,
            ParentDepartmentName = department.ParentDepartment?.Name,
            SubDepartments = new(),
            IsActive = department.IsActive
        };
    }
}
