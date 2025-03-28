using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models.Stream;

namespace Shared.Interfaces;

public interface IStreamService
{
    Task<StreamDto> GetStreamByIdAsync(Guid id);
    Task<IEnumerable<StreamDto>> GetActiveStreamsAsync();
    Task<StreamDto> GetStreamByUserIdAsync(Guid userId);
    Task<StreamDto> CreateStreamAsync(Guid? specifiedUserId = null);
    Task<StreamDto> UpdateStreamAsync(Guid id, StreamDto streamDto);
    Task<string> GenerateStreamKeyAsync(Guid id);
    Task StartStreamAsync(string streamKey);
    Task EndStreamAsync(string streamKey);
}
