using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models.Stream;
using Shared.Models.User;

namespace Shared.Interfaces;

public interface IRecommendationService
{
    Task<IEnumerable<StreamDto>> GetRecommendedStreamsAsync(Guid userId);
    Task<IEnumerable<UserDto>> GetRecommendedUsersToFollowAsync(Guid userId);
}
