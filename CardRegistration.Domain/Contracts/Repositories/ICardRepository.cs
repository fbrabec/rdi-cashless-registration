using CardRegistration.Domain.Entities;
using System.Threading.Tasks;

namespace CardRegistration.Domain.Contracts.Repositories
{
    public interface ICardRepository
    {
        Task<int> AddCardAsync(CardEntity card);

        Task<CardEntity> GetCardByIdAsync(int id);

        Task<CardEntity> GetCardByNumberAsync(long cardNumber);

        Task<int> AddTokenRegistrationAsync(TokenRegistrationEntity tokenRegistration);

        Task<TokenRegistrationEntity> GetRegistrationByCvvAsync(int cardId, int cvv);
    }
}
