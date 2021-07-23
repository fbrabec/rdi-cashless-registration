using CardRegistration.Application.Messages;
using System.Threading.Tasks;

namespace CardRegistration.Application.Services
{
    public interface ICardService
    {
        Task<RegisterCardResponse> RegisterCardAsync(RegisterCardRequest request);

        Task<ValidateTokenResponse> ValidateTokenAsync(int cardId, int customerId, long token, int CVV);
    }
}
