using Microsoft.AspNetCore.Authorization;

public class MinimumExperienceRequirement : IAuthorizationRequirement
{
    public int MinimumExperience { get; }

    public MinimumExperienceRequirement(int minimumExperience)
    {
        MinimumExperience = minimumExperience;
    }
}