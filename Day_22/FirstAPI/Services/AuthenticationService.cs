
public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly IEncryptionService _encryptionService;
    private readonly IRepository<string, User> _userRepository;
    private readonly ILogger<AuthenticationService> _logger;
    public AuthenticationService(ITokenService tokenService, IEncryptionService encryptionService, IRepository<string, User> userRepository, ILogger<AuthenticationService> logger)
    {
        _tokenService = tokenService;
        _encryptionService = encryptionService;
        _userRepository = userRepository;
        _logger = logger;
    }
    public async Task<UserLoginResponse> Login(UserLoginRequest user)
    {
        var normalizedEmail = user.Email.Trim().ToLower();
        var dbUser = await _userRepository.Get(normalizedEmail);
        if (dbUser == null)
        {
            _logger.LogCritical("Login failed for user {Email}: User not found", user.Email);
            throw new Exception("User not found");
        }

        var encryptedPassword = await _encryptionService.EncryptData(new EncryptionModel
        {
            Data = user.Password,
            HashKey = dbUser.HashKey
        });

        if (encryptedPassword?.EncryptedData == null || dbUser.Password == null ||
            !encryptedPassword.EncryptedData.SequenceEqual(dbUser.Password))
        {
            _logger.LogCritical("Login failed for user {Email}: Password mismatch", user.Email);
            throw new Exception("Password mismatch");
        }
        
        var token = await _tokenService.GenerateToken(dbUser);
        return new UserLoginResponse
        {
            Email = dbUser.Email,
            Token = token,
        };
    }
}