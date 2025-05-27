using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string ProfileUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<Tweet>? Tweets { get; set; }
    public ICollection<Like>? Likes { get; set; }

    [InverseProperty(nameof(Follow.Following))]
    public ICollection<Follow>? Followers { get; set; }

    [InverseProperty(nameof(Follow.Follower))]
    public ICollection<Follow>? Following { get; set; }
}