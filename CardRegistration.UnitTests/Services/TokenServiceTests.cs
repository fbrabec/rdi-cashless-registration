using CardRegistration.Domain.Contracts.Services;
using CardRegistration.Domain.Entities;
using CardRegistration.Domain.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace CardRegistration.UnitTests.Services
{
    public class TokenServiceTests
    {
        private ITokenService tokenService;

        public TokenServiceTests()
        {
            tokenService = new TokenService();
        }

        [Theory]
        [ClassData(typeof(TokenTestData))]
        public async void GenerateTokenAsync_RightCircularRotation(long expected, CardEntity card, int cvv)
        {
            var token = await tokenService.GenerateTokenAsync(card, cvv);

            Assert.Equal(expected, token);
        }

        [Theory]
        [ClassData(typeof(ValidateTokenTestData))]
        public async void ValidateTokenAsync_TokenIsEqual(bool expected, CardEntity card, long token, int cvv)
        {
            var valid = await tokenService.ValidateTokenAsync(card, token, cvv);

            Assert.Equal(expected, valid);
        }

        [Theory]
        [ClassData(typeof(ValidateTokenExpirationTestData))]
        public async void ValidateTokenExpirationAsync_TokenIsValid(bool expected, TokenRegistrationEntity tokenRegistration)
        {
            var valid = await tokenService.ValidateTokenExpirationAsync(tokenRegistration);

            Assert.Equal(expected, valid);
        }
    }

    public class TokenTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { (long)3412, new CardEntity() { CardNumber = (long)11111111234 }, 2 };
            yield return new object[] { (long)4123, new CardEntity() { CardNumber = (long)11111111234 }, 1 };
            yield return new object[] { (long)1234, new CardEntity() { CardNumber = (long)11111111234 }, 0 };
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
            yield return new object[] { true, new CardEntity() { CardNumber = 11111111234 }, (long)3412, 2 };
            yield return new object[] { false, new CardEntity() { CardNumber = 11111111234 }, (long)1234, 2 };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ValidateTokenExpirationTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { false, null };
            yield return new object[] { true, new TokenRegistrationEntity() { CreationDate = DateTime.UtcNow } };
            yield return new object[] { true, new TokenRegistrationEntity() { CreationDate = DateTime.UtcNow.AddMinutes(-10) } };
            yield return new object[] { true, new TokenRegistrationEntity() { CreationDate = DateTime.UtcNow.AddMinutes(-20) } };
            yield return new object[] { true, new TokenRegistrationEntity() { CreationDate = DateTime.UtcNow.AddMinutes(-29) } };
            yield return new object[] { false, new TokenRegistrationEntity() { CreationDate = DateTime.UtcNow.AddMinutes(-30) } };
            yield return new object[] { false, new TokenRegistrationEntity() { CreationDate = DateTime.UtcNow.AddMinutes(-31) } };
            yield return new object[] { false, new TokenRegistrationEntity() { CreationDate = DateTime.UtcNow.AddMinutes(-60) } };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
