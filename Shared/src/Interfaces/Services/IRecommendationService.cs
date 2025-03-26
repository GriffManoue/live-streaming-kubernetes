
using Shared.Models.Stream;

namespace Shared.Interfaces;

public interface IRecommendationService
{
    Task<IEnumerable<StreamDto>> GetRecommendedStreamsAsync(Guid userId);
}
