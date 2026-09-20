using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface ISliderService
{
    Task<IDataResult<IEnumerable<SliderDto>>> GetAllAsync(CancellationToken ct = default);
    Task<IDataResult<IEnumerable<SliderDto>>> GetPublishedAsync(CancellationToken ct = default);
    Task<IDataResult<SliderDto>> GetByIdAsync(int id);
    Task<IDataResult<SliderDto>> CreateAsync(SaveSliderDto dto);
    Task<IResult> UpdateAsync(int id, SaveSliderDto dto);
    Task<IResult> DeleteAsync(int id);
}
