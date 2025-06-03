public interface IAuthenticationService
{
    public Task<UserLoginResponse> Login(UserLoginRequest user);
}