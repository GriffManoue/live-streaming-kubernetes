using System;
using System.Threading.Tasks;
using Shared.Models.Analytics;

namespace Shared.Interfaces;

public interface IAnalyticsService
{
    Task RecordStreamViewAsync(Guid streamId, Guid? userId);
    Task<StreamAnalyticsDto> GetStreamAnalyticsAsync(Guid streamId);
    Task<UserAnalyticsDto> GetUserAnalyticsAsync(Guid userId);
}
