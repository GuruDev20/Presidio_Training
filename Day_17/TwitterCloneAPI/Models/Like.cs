using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Like
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    [ForeignKey("Tweet")]
    public int TweetId { get; set; }
    public Tweet Tweet { get; set; } = null!;
}