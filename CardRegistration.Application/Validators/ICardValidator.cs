using CardRegistration.Application.Messages;
using System.Threading.Tasks;

namespace CardRegistration.Application.Validators
{
    public interface ICardValidator
    {
        Task<RegisterCardResponse> ValidateAsync(RegisterCardRequest request);

        Task<ValidateTokenResponse> ValidateAsync(int cardId, int customerId, long token, int CVV);
    }
}
