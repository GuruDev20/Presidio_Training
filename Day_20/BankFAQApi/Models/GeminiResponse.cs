public class GeminiResponse
{
    public List<GeminiCandidate>? candidates { get; set; }
}

public class GeminiCandidate
{
    public GeminiContent? content { get; set; }
}

public class GeminiContent
{
    public List<GeminiPart>? parts { get; set; }
}

public class GeminiPart
{
    public string text { get; set; }= string.Empty;
}
