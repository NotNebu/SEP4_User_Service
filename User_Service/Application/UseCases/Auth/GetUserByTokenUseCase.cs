using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases
{
    public class GetUserByTokenUseCase : IGetUserByTokenUseCase
    {
        private readonly IJwtTokenGenerator _tokenGenerator;

        public GetUserByTokenUseCase(IJwtTokenGenerator tokenGenerator)
        {
            _tokenGenerator = tokenGenerator;
        }

        public Task<User?> ExecuteAsync(string token)
        {
            var user = _tokenGenerator.ValidateToken(token);
            return Task.FromResult(user);
        }
    }
}
