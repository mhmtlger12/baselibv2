using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

namespace Baselib.Business.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IRepository<AuditLog> _auditLogs;
    private readonly IUnitOfWork _unitOfWork;

    public AuditLogService(IRepository<AuditLog> auditLogs, IUnitOfWork unitOfWork)
    {
        _auditLogs = auditLogs;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
    {
        var logs = await _auditLogs.Query()
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedDate)
            .Take(500) // Sadece son 500 kaydı getir, performansı yormasın.
            .ToListAsync();

        return logs.Select(a => new AuditLogDto
        {
            Id = a.Id,
            UserId = a.UserId,
            Username = a.User?.Username,
            Action = a.Action,
            Controller = a.Controller,
            Route = a.Route,
            Details = a.Details,
            CreatedDate = a.CreatedDate
        });
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
            CreatedDate = DateTime.UtcNow
        };

        await _auditLogs.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
    }
}
