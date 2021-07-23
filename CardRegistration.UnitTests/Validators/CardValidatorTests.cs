using CardRegistration.Application.Impl.Validators;
using CardRegistration.Application.Messages;
using CardRegistration.Application.Validators;
using CardRegistration.Infrastructure.Common.Constants;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace CardRegistration.UnitTests.Validators
{
    public class CardValidatorTests
    {
        private ICardValidator cardValidator;

        public CardValidatorTests()
        {
            cardValidator = new CardValidator();
        }

        [Theory]
        [ClassData(typeof(CardTestData))]
        public async void RegisterCardAsync_ValidateInput(string expected, RegisterCardRequest request)
        {
            var response = await cardValidator.ValidateAsync(request);

            Assert.Equal(expected, response.Error?.Code);
        }

        [Theory]
        [ClassData(typeof(ValidateTokenTestData))]
        public async void ValidateTokenAsync_ValidateInput(string expected, int cardId, int customerId, long token, int cvv)
        {
            var response = await cardValidator.ValidateAsync(cardId, customerId, token, cvv);

            Assert.Equal(expected, response.Error?.Code);
        }
    }

    public class CardTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { Message.Code.CARD_E001.ToString(), null };
            yield return new object[] { null, new RegisterCardRequest() { CardNumber = 1723772873282736, CustomerId = 2, CVV = 12345 } };
            yield return new object[] { Message.Code.CARD_V013.ToString(), new RegisterCardRequest() { CardNumber = 1, CustomerId = 2, CVV = 12345 } };
            yield return new object[] { Message.Code.CARD_V001.ToString(), new RegisterCardRequest() { CardNumber = 17237728732827368, CustomerId = 2, CVV = 12345 } };
            yield return new object[] { Message.Code.CARD_V002.ToString(), new RegisterCardRequest() { CardNumber = 1723772873282736, CustomerId = 2, CVV = 123456 } };
            yield return new object[] { Message.Code.CARD_V003.ToString(), new RegisterCardRequest() { CardNumber = 1723772873282736, CustomerId = 0, CVV = 12345 } };
            yield return new object[] { Message.Code.CARD_V004.ToString(), new RegisterCardRequest() { CardNumber = 0, CustomerId = 2, CVV = 12345 } };
            yield return new object[] { Message.Code.CARD_V005.ToString(), new RegisterCardRequest() { CardNumber = 1723772873282736, CustomerId = 2, CVV = 0 } };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ValidateTokenTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { null, 1, 2, (long)4123, 12345 };
            yield return new object[] { Message.Code.CARD_V006.ToString(), 0, 2, (long)4123, 12345 };
            yield return new object[] { Message.Code.CARD_V003.ToString(), 1, 0, (long)4123, 12345 };
            yield return new object[] { Message.Code.CARD_V007.ToString(), 1, 2, (long)0, 12345 };
            yield return new object[] { Message.Code.CARD_V005.ToString(), 1, 2, (long)4123, 0 };
            yield return new object[] { Message.Code.CARD_V002.ToString(), 1, 2, (long)4123, 123456 };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}