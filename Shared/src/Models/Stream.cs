using Shared.models.Enums;

namespace Shared.models;

public class Stream
{
    User User { get; set; }
    string StreamName { get; set; }
    string StreamDescription { get; set; }
    StreamCategory StreamCategory { get; set; }
    
}