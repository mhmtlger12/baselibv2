using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Entities;

namespace Baselib.Business.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── Permission ────────────────────────────────────────────
        CreateMap<Permission, PermissionDto>();

        // ── Role ──────────────────────────────────────────────────
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.Permissions,
                opt => opt.MapFrom(src => src.RolePermissions.Select(rp => rp.Permission)));

        // ── User ──────────────────────────────────────────────────
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.DepartmentName,
                opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
            .ForMember(dest => dest.Roles,
                opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()))
            .ForMember(dest => dest.RoleIds,
                opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.RoleId).ToList()))
            .ForMember(dest => dest.ActiveRoleId, opt => opt.Ignore())
            .ForMember(dest => dest.ActiveRoleName, opt => opt.Ignore());

        // ── Department ────────────────────────────────────────────
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.ParentDepartmentName,
                opt => opt.MapFrom(src => src.ParentDepartment != null ? src.ParentDepartment.Name : null))
            .ForMember(dest => dest.SubDepartments, opt => opt.Ignore());

        // ── Menu ──────────────────────────────────────────────────
        CreateMap<Menu, MenuDto>()
            .ForMember(dest => dest.PermissionCode,
                opt => opt.MapFrom(src => src.Permission != null ? src.Permission.Code : null))
            .ForMember(dest => dest.SubMenus, opt => opt.Ignore());

        // ── AuditLog ──────────────────────────────────────────────
        CreateMap<AuditLog, AuditLogDto>()
            .ForMember(dest => dest.Username,
                opt => opt.MapFrom(src => src.User != null ? src.User.Username : null));

        // ── AppSetting ────────────────────────────────────────────
        CreateMap<AppSetting, SettingDto>();
    }
}
