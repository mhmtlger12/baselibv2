using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface ISliderService
{
    Task<IDataResult<IEnumerable<SliderDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<IEnumerable<SliderDto>>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<SliderDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IDataResult<SliderDto>> CreateAsync(SaveSliderDto dto, CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(int id, SaveSliderDto dto, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
