using System;
using System.Threading.Tasks;
using Shared.Models.Analytics;

namespace Shared.Interfaces;

public interface IAnalyticsService
{
    Task RecordStreamViewAsync(Guid streamId, Guid? userId);
    Task<StreamAnalyticsDto> GetStreamAnalyticsAsync(Guid streamId);
    Task<UserAnalyticsDto> GetUserAnalyticsAsync(Guid userId);
    Task TrackStreamViewAsync(Guid streamId, Guid userId, DateTime timestamp);
    Task TrackStreamEngagementAsync(Guid streamId, Guid userId, string engagementType, DateTime timestamp);
}
