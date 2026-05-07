using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

namespace Baselib.Business.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IRepository<Department> _departments;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DepartmentService(IRepository<Department> departments, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _departments = departments;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        var departments = await _departments.Query()
            .Include(d => d.ParentDepartment)
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();

        return departments.Select(d => _mapper.Map<DepartmentDto>(d));
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

        return department == null ? null : _mapper.Map<DepartmentDto>(department);
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
    {
        await EnsureCodeIsUniqueAsync(dto.Code);

        var department = new Department
        {
            Name = dto.Name.Trim(),
            Code = dto.Code.Trim(),
            ParentDepartmentId = dto.ParentDepartmentId,
            CreatedDate = DateTime.UtcNow,
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
            throw new KeyNotFoundException(Messages.Department.NotFound);

        if (dto.ParentDepartmentId == id)
            throw new InvalidOperationException(Messages.General.SelfReferenceNotAllowed);

        await EnsureCodeIsUniqueAsync(dto.Code, id);

        department.Name = dto.Name.Trim();
        department.Code = dto.Code.Trim();
        department.ParentDepartmentId = dto.ParentDepartmentId;
        department.IsActive = dto.IsActive;
        department.UpdatedDate = DateTime.UtcNow;

        await _departments.UpdateAsync(department);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _departments.SoftDeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task EnsureCodeIsUniqueAsync(string code, int? departmentId = null)
    {
        code = code.Trim();

        if (await _departments.AnyAsync(d => d.Code == code && (!departmentId.HasValue || d.Id != departmentId.Value)))
            throw new InvalidOperationException(Messages.Department.CodeAlreadyExists);
    }

    private List<DepartmentDto> BuildTree(IEnumerable<Department> departments, int? parentId)
    {
        return departments
            .Where(d => d.ParentDepartmentId == parentId)
            .Select(d =>
            {
                var dto = _mapper.Map<DepartmentDto>(d);
                dto.SubDepartments = BuildTree(departments, d.Id);
                return dto;
            })
            .ToList();
    }
}
