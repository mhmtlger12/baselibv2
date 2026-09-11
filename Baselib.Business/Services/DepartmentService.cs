using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;

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

    public async Task<IDataResult<IEnumerable<DepartmentDto>>> GetAllAsync()
    {
        var departments = await _departments.GetAllAsync(
            includes: [d => d.ParentDepartment!]);

        return DataResult<IEnumerable<DepartmentDto>>.Ok(
            departments.OrderBy(d => d.Name).Select(d => _mapper.Map<DepartmentDto>(d)));
    }

    public async Task<IDataResult<IEnumerable<DepartmentDto>>> GetTreeAsync()
    {
        var departments = await _departments.GetAllAsync();

        return DataResult<IEnumerable<DepartmentDto>>.Ok(
            BuildTree(departments.OrderBy(d => d.Name), null));
    }

    public async Task<IDataResult<DepartmentDto>> GetByIdAsync(int id)
    {
        var department = await _departments.GetByIdAsync(
            id,
            d => d.ParentDepartment!,
            d => d.SubDepartments);

        if (department == null)
            return DataResult<DepartmentDto>.NotFound(Messages.Department.NotFound);

        return DataResult<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(department));
    }

    public async Task<IDataResult<DepartmentDto>> CreateAsync(CreateDepartmentDto dto)
    {
        var code = dto.Code.Trim();
        if (await _departments.AnyAsync(d => d.Code == code))
            return DataResult<DepartmentDto>.BadRequest(Messages.Department.CodeAlreadyExists);

        var department = new Department
        {
            Name = dto.Name.Trim(),
            Code = code,
            ParentDepartmentId = dto.ParentDepartmentId,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        await _departments.AddAsync(department);
        await _unitOfWork.SaveChangesAsync();

        var created = await GetByIdAsync(department.Id);
        return DataResult<DepartmentDto>.Created(created.Data!, Messages.General.Saved);
    }

    public async Task<IResult> UpdateAsync(int id, UpdateDepartmentDto dto)
    {
        var department = await _departments.GetByIdAsync(id);
        if (department == null)
            return Result.NotFound(Messages.Department.NotFound);

        if (dto.ParentDepartmentId == id)
            return Result.BadRequest(Messages.General.SelfReferenceNotAllowed);

        var code = dto.Code.Trim();
        if (await _departments.AnyAsync(d => d.Code == code && d.Id != id))
            return Result.BadRequest(Messages.Department.CodeAlreadyExists);

        department.Name = dto.Name.Trim();
        department.Code = code;
        department.ParentDepartmentId = dto.ParentDepartmentId;
        department.IsActive = dto.IsActive;
        department.UpdatedDate = DateTime.UtcNow;

        _departments.Update(department);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var department = await _departments.GetByIdAsync(id);
        if (department == null)
            return Result.NotFound(Messages.Department.NotFound);

        department.IsActive = false;
        department.UpdatedDate = DateTime.UtcNow;
        _departments.Update(department);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Deleted);
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
