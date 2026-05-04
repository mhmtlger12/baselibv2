using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(Policy = "DynamicPermission")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var users = await _userService.GetAllAsync();
        return Ok(DataResult<IEnumerable<UserDto>>.SuccessDataResult(users));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user == null 
            ? NotFound(Result.ErrorResult(Messages.User.NotFound, 404))
            : Ok(DataResult<UserDto>.SuccessDataResult(user));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserDto dto)
    {
        var user = await _userService.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = user.Id }, 
            DataResult<UserDto>.SuccessDataResult(user, Messages.General.Saved));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        await _userService.UpdateAsync(id, dto);
        return Ok(Result.SuccessResult(Messages.General.Updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _userService.DeleteAsync(id);
        return Ok(Result.SuccessResult(Messages.General.Deleted));
    }

    [HttpPut("{id}/roles")]
    public async Task<IActionResult> AssignRoles(int id, [FromBody] List<int> roleIds)
    {
        await _userService.AssignRolesAsync(id, roleIds);
        return Ok(Result.SuccessResult("Roller başarıyla atandı"));
    }
}