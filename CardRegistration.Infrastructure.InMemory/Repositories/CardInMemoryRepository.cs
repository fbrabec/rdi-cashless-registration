using CardRegistration.Domain.Contracts.Repositories;
using CardRegistration.Domain.Entities;
using CardRegistration.Infrastructure.DbContexts;
using System.Threading.Tasks;

namespace CardRegistration.Infrastructure.InMemory.Repositories
{
    public class CardInMemoryRepository : ICardRepository
    {
        private readonly CustomerCardContext customerCardContext;

        public CardInMemoryRepository(CustomerCardContext customerCardContext)
        {
            this.customerCardContext = customerCardContext;
        }

        public async Task<int> AddCardAsync(CardEntity card)
        {
            await customerCardContext.AddAsync(card);

            await customerCardContext.SaveChangesAsync();

            return card.Id;
        }

        public async Task<int> AddTokenRegistrationAsync(TokenRegistrationEntity tokenRegistration)
        {
            await customerCardContext.AddAsync(tokenRegistration);

            await customerCardContext.SaveChangesAsync();

            return tokenRegistration.Id;
        }

        public async Task<CardEntity> GetCardByIdAsync(int id)
        {
            return await customerCardContext.FindAsync<CardEntity>(id);
        }

        public async Task<CardEntity> GetCardByNumberAsync(long cardNumber)
        {
            return await customerCardContext.GetCardByNumber(cardNumber);
        }

        public async Task<TokenRegistrationEntity> GetRegistrationByCvvAsync(int cardId, int cvv)
        {
            return await customerCardContext.GetRegistrationByCvvAsync(cardId, cvv);
        }
    }
}
