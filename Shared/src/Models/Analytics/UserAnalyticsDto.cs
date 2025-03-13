using System;
using System.Collections.Generic;

namespace Shared.Models.Analytics;

public class UserAnalyticsDto
{
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public int TotalStreamCount { get; set; }
    public int TotalStreamViews { get; set; }
    public int TotalUniqueViewers { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public TimeSpan TotalStreamTime { get; set; }
    public Dictionary<string, int> ViewsByCategory { get; set; } = new();
    public List<StreamViewSummary> TopStreams { get; set; } = new();
}

public class StreamViewSummary
{
    public Guid StreamId { get; set; }
    public string? StreamName { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
