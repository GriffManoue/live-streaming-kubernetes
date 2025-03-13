using System;
using System.Collections.Generic;

namespace Shared.Models.Analytics;

public class StreamAnalyticsDto
{
    public Guid StreamId { get; set; }
    public string? StreamName { get; set; }
    public int TotalViews { get; set; }
    public int UniqueViewers { get; set; }
    public int PeakViewerCount { get; set; }
    public TimeSpan AverageWatchTime { get; set; }
    public Dictionary<string, int> ViewersByCountry { get; set; } = new();
    public Dictionary<string, int> ViewersByDevice { get; set; } = new();
}
