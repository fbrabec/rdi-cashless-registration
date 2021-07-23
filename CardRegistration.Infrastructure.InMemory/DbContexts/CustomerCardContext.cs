using CardRegistration.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CardRegistration.Infrastructure.DbContexts
{
    public class CustomerCardContext : DbContext
    {
        public CustomerCardContext (DbContextOptions<CustomerCardContext> options)
            : base(options)
        {
        }

        public DbSet<CardEntity> Cards { get; set; }
        public DbSet<TokenRegistrationEntity> TokenRegistrations { get; set; }

        public async Task<CardEntity> GetCardByNumber(long cardNumber)
        {
            return await Cards.FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
        }

        public async Task<TokenRegistrationEntity> GetRegistrationByCvvAsync(int cardId, int cvv)
        {
            return await TokenRegistrations.FirstOrDefaultAsync(t => t.CardId == cardId && t.CVV == cvv);
        }
    }
}
