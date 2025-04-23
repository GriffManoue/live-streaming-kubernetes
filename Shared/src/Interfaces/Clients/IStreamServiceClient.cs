using System;
using System.Threading.Tasks;
using Shared.Models.Domain;
using Shared.Models.Stream;

namespace StreamDbHandler.Services;

public interface IStreamServiceClient
{
    Task<StreamDto?> GetStreamByIdAsync(Guid streamId);
    Task<StreamDto> CreateStreamAsync(User? user = null);
}