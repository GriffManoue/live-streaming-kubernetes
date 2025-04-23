using System;
using System.Threading.Tasks;
using Shared.Models.Domain;
using Shared.Models.Stream;

namespace Shared.Interfaces.Clients;

public interface IStreamDbHandlerClient
{
    Task<StreamDto?> GetStreamByIdAsync(Guid id);
    Task<IEnumerable<StreamDto>> GetActiveStreamsAsync();
    Task<IEnumerable<StreamDto>> GetAllStreamsAsync();
    Task<StreamDto?> GetStreamByUserIdAsync(Guid userId);
    Task<StreamDto> CreateStreamAsync(Guid userId);
    Task<StreamDto?> UpdateStreamAsync(Guid id, StreamDto streamDto);
}