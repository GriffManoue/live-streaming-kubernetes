using System;

namespace Shared.src.Interfaces.Services;

public interface IViewerService
{

    Task JoinViewerAsync(Guid streamId, string viewerId);
    Task LeaveViewerAsync(Guid streamId, string viewerId);
    Task<int> GetViewerCountAsync(Guid streamId);

}
