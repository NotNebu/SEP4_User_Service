using SEP4_User_Service.Application.Interfaces;

namespace SEP4_User_Service.Application.UseCases
{
    public class RegisterUseCase : IRegisterUseCase
    {
        private readonly IAuthService _authService;

        public RegisterUseCase(IAuthService authService)
        {
            _authService = authService;
        }

        public Task<bool> ExecuteAsync(string email, string password, string username)
        {
            return _authService.RegisterAsync(email, password, username);
        }
    }
}