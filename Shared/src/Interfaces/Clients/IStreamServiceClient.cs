using Shared.Models.Stream;

namespace StreamService.Services;
public interface IStreamServiceClient
{
    Task<StreamDto?> GetStreamByIdAsync(Guid streamId);
}