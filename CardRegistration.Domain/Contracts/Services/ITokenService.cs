using CardRegistration.Domain.Entities;
using System.Threading.Tasks;

namespace CardRegistration.Domain.Contracts.Services
{
    public interface ITokenService
    {
        Task<long> GenerateTokenAsync(CardEntity cardEntity, int cvv);
        Task<bool> ValidateTokenAsync(CardEntity cardEntity, long token, int cvv);
        Task<bool> ValidateTokenExpirationAsync(TokenRegistrationEntity tokenRegistration);
    }
}
