using System;
using System.Threading.Tasks;
using Shared.Models.Stream;

namespace StreamService.Services;

public interface IStreamServiceClient
{
    Task<StreamDto?> GetStreamByIdAsync(Guid streamId);
    Task<StreamDto> CreateStreamAsync();
}