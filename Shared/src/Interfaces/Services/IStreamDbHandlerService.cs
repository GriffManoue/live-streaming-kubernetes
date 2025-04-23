using Shared.Models.Stream;

namespace Shared.Interfaces;

public interface IStreamDbHandlerService
{
    Task<StreamDto> GetStreamByIdAsync(Guid id);
    Task<IEnumerable<StreamDto>> GetActiveStreamsAsync();

    Task<IEnumerable<StreamDto>> GetAllStreamsAsync();
    Task<StreamDto> GetStreamByUserIdAsync(Guid userId);
    Task<StreamDto> CreateStreamAsync(Guid? specifiedUserId = null);
    Task<StreamDto> UpdateStreamAsync(Guid id, StreamDto streamDto);
}
