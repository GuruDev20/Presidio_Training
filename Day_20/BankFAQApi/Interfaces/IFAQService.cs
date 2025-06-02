public interface IFAQService
{
    Task<string> GetAnswerAsync(string question);
}
