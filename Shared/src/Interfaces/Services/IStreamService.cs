using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models.Stream;

namespace Shared.Interfaces;

public interface IStreamService
{
    Task<StreamDto> GetStreamByIdAsync(Guid id);
    Task<IEnumerable<StreamDto>> GetActiveStreamsAsync();
    Task<IEnumerable<StreamDto>> GetStreamsByUserIdAsync(Guid userId);
    Task CreateStreamAsync();
    Task<StreamDto> UpdateStreamAsync(Guid id, UpdateStreamRequest request);
    Task EndStreamAsync(Guid id);
}
