using Microsoft.EntityFrameworkCore;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Data;
using Baselib.Entities;

namespace Baselib.Business.Services;

public class DepartmentService : IDepartmentService
{
    private readonly AppDbContext _context;

    public DepartmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        var departments = await _context.Departments
            .Include(d => d.ParentDepartment)
            .Where(d => d.IsActive)
            .ToListAsync();

        return departments.Select(MapToDto);
    }

    public async Task<IEnumerable<DepartmentDto>> GetTreeAsync()
    {
        var departments = await _context.Departments
            .Include(d => d.SubDepartments)
            .Where(d => d.ParentDepartmentId == null && d.IsActive)
            .ToListAsync();

        return departments.Select(MapTreeToDto);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var department = await _context.Departments
            .Include(d => d.ParentDepartment)
            .Include(d => d.SubDepartments)
            .FirstOrDefaultAsync(d => d.Id == id);

        return department == null ? null : MapToDto(department);
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
    {
        if (await _context.Departments.AnyAsync(d => d.Code == dto.Code))
            throw new Exception("Department code already exists");

        var department = new Department
        {
            Name = dto.Name,
            Code = dto.Code,
            ParentDepartmentId = dto.ParentDepartmentId,
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(department.Id))!;
    }

    public async Task UpdateAsync(int id, UpdateDepartmentDto dto)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) throw new Exception("Department not found");

        if (await _context.Departments.AnyAsync(d => d.Code == dto.Name && d.Id != id))
            throw new Exception("Department code already exists");

        department.Name = dto.Name;
        department.ParentDepartmentId = dto.ParentDepartmentId;
        department.IsActive = dto.IsActive;
        department.UpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) throw new Exception("Department not found");

        department.IsActive = false;
        department.UpdatedDate = DateTime.Now;
        await _context.SaveChangesAsync();
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

    private static DepartmentDto MapTreeToDto(Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            ParentDepartmentId = department.ParentDepartmentId,
            ParentDepartmentName = department.ParentDepartment?.Name,
            SubDepartments = department.SubDepartments
                .Where(d => d.IsActive)
                .Select(MapTreeToDto)
                .ToList(),
            IsActive = department.IsActive
        };
    }
}