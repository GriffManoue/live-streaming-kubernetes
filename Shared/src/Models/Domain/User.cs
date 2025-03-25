using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Shared.Models.Domain;

public partial class User
{

    public Guid Id { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public LiveStream Stream { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsLive { get; set; } = false;

    public ICollection<User> Followers{ get; set; } = new List<User>();

    public ICollection<User> Following { get; set; } = new List<User>();
}
