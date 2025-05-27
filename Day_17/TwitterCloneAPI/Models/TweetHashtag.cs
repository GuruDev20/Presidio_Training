using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TweetHashtag
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Tweet")]
    public int TweetId { get; set; }
    public Tweet Tweet { get; set; } = null!;

    [ForeignKey("Hashtag")]
    public int HashtagId { get; set; }
    public Hashtag Hashtag { get; set; } = null!;
}