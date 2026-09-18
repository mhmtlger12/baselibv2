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
    private readonly TimeProvider _timeProvider;

    public DepartmentService(IRepository<Department> departments, IUnitOfWork unitOfWork, IMapper mapper, TimeProvider timeProvider)
    {
        _departments = departments;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<IDataResult<IEnumerable<DepartmentDto>>> GetAllAsync()
    {
        var departments = await _departments.GetAllAsync(
            asNoTracking: true,
            includes: [d => d.ParentDepartment!]);

        return DataResult<IEnumerable<DepartmentDto>>.Ok(
            departments.OrderBy(d => d.Name).Select(d => _mapper.Map<DepartmentDto>(d)));
    }

    public async Task<IDataResult<IEnumerable<SelectOptionDto>>> GetSelectOptionsAsync()
    {
        var departments = await _departments.GetAllAsync(asNoTracking: true);
        var options = departments.OrderBy(d => d.Name).Select(d => new SelectOptionDto
        {
            Id = d.Id,
            Name = d.Name
        });

        return DataResult<IEnumerable<SelectOptionDto>>.Ok(options);
    }

    public async Task<IDataResult<IEnumerable<DepartmentDto>>> GetTreeAsync()
    {
        var departments = await _departments.GetAllAsync(asNoTracking: true);

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
        if (await _departments.AnyAsync(d => d.Code == code, ignoreQueryFilters: true))
            return DataResult<DepartmentDto>.BadRequest(Messages.Department.CodeAlreadyExists);

        var parentValidation = await ValidateParentAsync(dto.ParentDepartmentId, null);
        if (!parentValidation.Success)
            return DataResult<DepartmentDto>.ErrorDataResult(parentValidation.Message, parentValidation.StatusCode);

        var department = new Department
        {
            Name = dto.Name.Trim(),
            Code = code,
            ParentDepartmentId = dto.ParentDepartmentId,
            CreatedDate = _timeProvider.GetUtcNow().UtcDateTime,
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

        var parentValidation = await ValidateParentAsync(dto.ParentDepartmentId, id);
        if (!parentValidation.Success)
            return parentValidation;

        var code = dto.Code.Trim();
        if (await _departments.AnyAsync(d => d.Code == code && d.Id != id, ignoreQueryFilters: true))
            return Result.BadRequest(Messages.Department.CodeAlreadyExists);

        department.Name = dto.Name.Trim();
        department.Code = code;
        department.ParentDepartmentId = dto.ParentDepartmentId;
        department.IsActive = dto.IsActive;
        department.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;

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
        department.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;
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

    private async Task<IResult> ValidateParentAsync(int? parentDepartmentId, int? departmentId)
    {
        if (!parentDepartmentId.HasValue)
            return Result.Ok();

        var visitedDepartmentIds = new HashSet<int>();
        var currentParentId = parentDepartmentId;

        while (currentParentId.HasValue)
        {
            if (departmentId == currentParentId || !visitedDepartmentIds.Add(currentParentId.Value))
                return Result.BadRequest(Messages.General.HierarchyCycleNotAllowed);

            var parent = await _departments.GetByIdAsync(currentParentId.Value);
            if (parent == null)
                return Result.BadRequest(Messages.General.ParentNotFoundOrInactive);

            currentParentId = parent.ParentDepartmentId;
        }

        return Result.Ok();
    }
}
