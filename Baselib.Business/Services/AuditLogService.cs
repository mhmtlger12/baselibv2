using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Results;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

namespace Baselib.Business.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IRepository<AuditLog> _auditLogs;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public AuditLogService(IRepository<AuditLog> auditLogs, IUnitOfWork unitOfWork, IMapper mapper, TimeProvider timeProvider)
    {
        _auditLogs = auditLogs;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<IDataResult<IEnumerable<AuditLogDto>>> GetAllAsync()
    {
        var logs = await _auditLogs.GetAllAsync(
            predicate: null,
            include: q => q.Include(a => a.User).OrderByDescending(a => a.CreatedDate).Take(500),
            asNoTracking: true);

        return DataResult<IEnumerable<AuditLogDto>>.Ok(_mapper.Map<IEnumerable<AuditLogDto>>(logs));
    }

    public async Task LogAsync(int? userId, string action, string controller, string route, string? details)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action = action,
            Controller = controller,
            Route = route,
            Details = details,
            CreatedDate = _timeProvider.GetUtcNow().UtcDateTime
        };

        await _auditLogs.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
    }
}
