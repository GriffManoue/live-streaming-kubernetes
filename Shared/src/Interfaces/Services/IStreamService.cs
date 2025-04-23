using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models.Stream;

namespace Shared.Interfaces;

public interface IStreamService
{
    Task<string> GenerateStreamKeyAsync(Guid id);
    Task StartStreamAsync(string streamKey);
    Task EndStreamAsync(string streamKey);
    Task<IEnumerable<StreamDto>> GetReccommendedStreamsAsync(Guid userId, int count = 6);

}
