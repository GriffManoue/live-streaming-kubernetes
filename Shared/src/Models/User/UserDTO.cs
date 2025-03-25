using System.ComponentModel.DataAnnotations;



public  class UserDTO
{

     public Guid Id { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public Guid StreamId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsLive { get; set; } = false;

    public ICollection<Guid> FollowerIds{ get; set; } = new List<Guid>();

    public ICollection<Guid> FollowingIds { get; set; } = new List<Guid>();

   
}