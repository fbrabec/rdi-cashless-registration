using CardRegistration.Domain.Contracts.Services;
using CardRegistration.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CardRegistration.Domain.Services
{
    public class TokenService : ITokenService
    {
        public async Task<long> GenerateTokenAsync(CardEntity cardEntity, int cvv)
        {
            var cardNumber = cardEntity.CardNumber.ToString();
            var last4Digits = cardNumber.Substring(cardNumber.Length - 4);

            int[] array = last4Digits.Select(c => (int)Char.GetNumericValue(c)).ToArray();

            for (int x = 0; x < cvv; x++)
                MoveInRightCircularRotation(array);

            var stringToken = string.Concat(array.Select(i => i.ToString()));

            var token = long.Parse(stringToken);

            return token;
        }

        public async Task<bool> ValidateTokenAsync(CardEntity cardEntity, long token, int cvv)
        {
            if (cardEntity == null)
                return false;

            var calculatedToken = await GenerateTokenAsync(cardEntity, cvv);

            return (calculatedToken == token);
        }

        public async Task<bool> ValidateTokenExpirationAsync(TokenRegistrationEntity tokenRegistration)
        {
            if (tokenRegistration == null)
                return false;

            TimeSpan timeSpan = DateTime.UtcNow - tokenRegistration.CreationDate;

            return (timeSpan.TotalMinutes <= 30);
        }

        #region Private methods
        private void MoveInRightCircularRotation(int[] array)
        {
            int temp;

            for (int i = 0; i < array.Length - 1; i++)
            {
                temp = array[0];
                array[0] = array[i + 1];
                array[i + 1] = temp;
            }
        }
        #endregion
    }
}
