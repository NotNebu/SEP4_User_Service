using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases
{
    public class GetUserByTokenUseCase : IGetUserByTokenUseCase
    {
        private readonly IAuthService _authService;

        public GetUserByTokenUseCase(IAuthService authService)
        {
            _authService = authService;
        }

        public Task<User?> ExecuteAsync(string token)
        {
            return _authService.GetUserByTokenAsync(token);
        }
    }
}