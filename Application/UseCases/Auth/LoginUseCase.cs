using SEP4_User_Service.Application.Interfaces;

namespace SEP4_User_Service.Application.UseCases
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IAuthService _authService;

        public LoginUseCase(IAuthService authService)
        {
            _authService = authService;
        }

        public Task<string> ExecuteAsync(string email, string password)
        {
            return _authService.LoginAsync(email, password);
        }
    }
}