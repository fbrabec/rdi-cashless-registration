using AutoMapper;
using CardRegistration.Application.Impl.Services;
using CardRegistration.Application.Messages;
using CardRegistration.Application.Validators;
using CardRegistration.Domain.Contracts.Repositories;
using CardRegistration.Domain.Contracts.Services;
using CardRegistration.Domain.Entities;
using CardRegistration.Infrastructure.Common.Constants;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CardRegistration.UnitTests.Services
{
    public class CardServiceTests
    {
        [Fact]
        public async void RegisterCard_ShouldReturn_ValidationError()
        {
            var cardValidator = new Mock<ICardValidator>();

            cardValidator.Setup(x => x.ValidateAsync(It.IsAny<RegisterCardRequest>()))
                .Returns(Task.FromResult(new RegisterCardResponse() { Error = new Application.Dtos.ErrorDto { Code = Message.Code.CARD_V001.ToString() } }));

            var cardService = new CardService(cardValidator.Object, null, null, null, null);

            var response = await cardService.RegisterCardAsync(new RegisterCardRequest());

            Assert.Equal(Message.Code.CARD_V001.ToString(), response.Error?.Code);
        }

        [Fact]
        public async void RegisterCard_ShouldReturn_Token_CardId_RegistrationDate()
        {
            var cardValidator = new Mock<ICardValidator>();
            cardValidator.Setup(x => x.ValidateAsync(It.IsAny<RegisterCardRequest>())).Returns(Task.FromResult(new RegisterCardResponse()));

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(x => x.GenerateTokenAsync(It.IsAny<CardEntity>(), It.IsAny<int>())).Returns(Task.FromResult((long)37873));

            var cardRepository = new Mock<ICardRepository>();
            cardRepository.Setup(x => x.AddCardAsync(It.IsAny<CardEntity>())).Returns(Task.FromResult(1));

            var cardService = new CardService(cardValidator.Object, tokenService.Object, cardRepository.Object,
                new Mock<IMapper>().Object, new Mock<ILogger<CardService>>().Object);

            var response = await cardService.RegisterCardAsync(new RegisterCardRequest());

            Assert.Equal(37873, response.Token);
            Assert.Equal(1, response.CardId);
            Assert.Equal(DateTime.UtcNow.Date, response.RegistrationDate.Date);
            Assert.Null(response.Error);
        }

        [Fact]
        public async void RegisterCard_ShouldReturn_Error_If_Cvv_Already_Registered()
        {
            var cardValidator = new Mock<ICardValidator>();
            cardValidator.Setup(x => x.ValidateAsync(It.IsAny<RegisterCardRequest>())).Returns(Task.FromResult(new RegisterCardResponse()));

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(x => x.GenerateTokenAsync(It.IsAny<CardEntity>(), It.IsAny<int>())).Returns(Task.FromResult((long)37873));

            var cardRepository = new Mock<ICardRepository>();
            cardRepository.Setup(x => x.GetCardByNumberAsync(It.IsAny<long>())).Returns(Task.FromResult(new CardEntity() { Id = 1 }));
            cardRepository.Setup(x => x.GetRegistrationByCvvAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(new TokenRegistrationEntity()));

            var cardService = new CardService(cardValidator.Object, tokenService.Object, cardRepository.Object,
                new Mock<IMapper>().Object, new Mock<ILogger<CardService>>().Object);

            var response = await cardService.RegisterCardAsync(new RegisterCardRequest());

            Assert.NotNull(response.Error);
            Assert.Equal(Message.Code.CARD_V010.ToString(), response.Error?.Code);
        }

        [Fact]
        public async void ValidateToken_ShouldReturn_ValidationError()
        {
            var cardValidator = new Mock<ICardValidator>();

            cardValidator.Setup(x => x.ValidateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new ValidateTokenResponse() { Error = new Application.Dtos.ErrorDto { Code = Message.Code.CARD_V001.ToString() } }));

            var cardService = new CardService(cardValidator.Object, null, null, null, null);

            var response = await cardService.ValidateTokenAsync(1, 1, (long)1234, 1);

            Assert.Equal(Message.Code.CARD_V001.ToString(), response.Error?.Code);
        }

        [Fact]
        public async void ValidateToken_ShouldReturn_Error_If_Card_Not_Found()
        {
            var cardValidator = new Mock<ICardValidator>();
            cardValidator.Setup(x => x.ValidateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new ValidateTokenResponse()));

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(x => x.GenerateTokenAsync(It.IsAny<CardEntity>(), It.IsAny<int>())).Returns(Task.FromResult((long)37873));

            var cardRepository = new Mock<ICardRepository>();
            cardRepository.Setup(x => x.GetCardByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(default(CardEntity)));

            var cardService = new CardService(cardValidator.Object, tokenService.Object, cardRepository.Object,
                new Mock<IMapper>().Object, new Mock<ILogger<CardService>>().Object);

            var response = await cardService.ValidateTokenAsync(1, 1, (long)1234, 1);

            Assert.NotNull(response.Error);
            Assert.Equal(Message.Code.CARD_V014.ToString(), response.Error?.Code);
        }

        [Fact]
        public async void ValidateToken_ShouldReturn_Error_If_Registration_Not_Found()
        {
            var cardValidator = new Mock<ICardValidator>();
            cardValidator.Setup(x => x.ValidateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new ValidateTokenResponse()));

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(x => x.GenerateTokenAsync(It.IsAny<CardEntity>(), It.IsAny<int>())).Returns(Task.FromResult((long)37873));

            var cardRepository = new Mock<ICardRepository>();
            cardRepository.Setup(x => x.GetCardByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new CardEntity() { CardNumber = 11111111111 }));
            cardRepository.Setup(x => x.GetRegistrationByCvvAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(default(TokenRegistrationEntity)));

            var cardService = new CardService(cardValidator.Object, tokenService.Object, cardRepository.Object,
                new Mock<IMapper>().Object, new Mock<ILogger<CardService>>().Object);

            var response = await cardService.ValidateTokenAsync(1, 1, (long)1234, 1);

            Assert.NotNull(response.Error);
            Assert.Equal(Message.Code.CARD_V012.ToString(), response.Error?.Code);
        }

        [Fact]
        public async void ValidateToken_ShouldReturn_Valid()
        {
            var cardValidator = new Mock<ICardValidator>();
            cardValidator.Setup(x => x.ValidateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new ValidateTokenResponse()));

            var cardRepository = new Mock<ICardRepository>();
            cardRepository.Setup(x => x.GetCardByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new CardEntity() { CustomerId = 1, CardNumber = 11111111234 }));
            cardRepository.Setup(x => x.GetRegistrationByCvvAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new TokenRegistrationEntity { CreationDate = DateTime.UtcNow.AddDays(-10) }));

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(x => x.ValidateTokenAsync(It.IsAny<CardEntity>(), It.IsAny<long>(), It.IsAny<int>())).Returns(Task.FromResult(true));
            tokenService.Setup(x => x.ValidateTokenExpirationAsync(It.IsAny<TokenRegistrationEntity>())).Returns(Task.FromResult(true));

            var cardService = new CardService(cardValidator.Object, tokenService.Object, cardRepository.Object,
                new Mock<IMapper>().Object, new Mock<ILogger<CardService>>().Object);

            var response = await cardService.ValidateTokenAsync(1, 1, (long)3412, 2);

            Assert.Null(response.Error);
            Assert.True(response.Validated);
        }

        [Fact]
        public async void ValidateToken_ShouldReturn_Invalid_Wrong_Token()
        {
            var cardValidator = new Mock<ICardValidator>();
            cardValidator.Setup(x => x.ValidateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new ValidateTokenResponse()));

            var cardRepository = new Mock<ICardRepository>();
            cardRepository.Setup(x => x.GetCardByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new CardEntity() { CustomerId = 1, CardNumber = 11111111234 }));
            cardRepository.Setup(x => x.GetRegistrationByCvvAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new TokenRegistrationEntity { CreationDate = DateTime.UtcNow.AddDays(-10) }));

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(x => x.ValidateTokenAsync(It.IsAny<CardEntity>(), It.IsAny<long>(), It.IsAny<int>())).Returns(Task.FromResult(false));
            tokenService.Setup(x => x.ValidateTokenExpirationAsync(It.IsAny<TokenRegistrationEntity>())).Returns(Task.FromResult(true));

            var cardService = new CardService(cardValidator.Object, tokenService.Object, cardRepository.Object,
                new Mock<IMapper>().Object, new Mock<ILogger<CardService>>().Object);

            var response = await cardService.ValidateTokenAsync(1, 1, (long)1234, 2);

            Assert.Null(response.Error);
            Assert.False(response.Validated);
        }

        [Fact]
        public async void ValidateToken_ShouldReturn_Invalid_Token_Expired()
        {
            var cardValidator = new Mock<ICardValidator>();
            cardValidator.Setup(x => x.ValidateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new ValidateTokenResponse()));

            var cardRepository = new Mock<ICardRepository>();
            cardRepository.Setup(x => x.GetCardByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new CardEntity() { CustomerId = 1, CardNumber = 11111111234 }));
            cardRepository.Setup(x => x.GetRegistrationByCvvAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new TokenRegistrationEntity { CreationDate = DateTime.UtcNow.AddDays(-40) }));

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(x => x.ValidateTokenAsync(It.IsAny<CardEntity>(), It.IsAny<long>(), It.IsAny<int>())).Returns(Task.FromResult(true));
            tokenService.Setup(x => x.ValidateTokenExpirationAsync(It.IsAny<TokenRegistrationEntity>())).Returns(Task.FromResult(false));


            var cardService = new CardService(cardValidator.Object, tokenService.Object, cardRepository.Object,
                new Mock<IMapper>().Object, new Mock<ILogger<CardService>>().Object);

            var response = await cardService.ValidateTokenAsync(1, 1, (long)3412, 2);

            Assert.Null(response.Error);
            Assert.False(response.Validated);
        }

        [Fact]
        public async void ValidateToken_ShouldReturn_Invalid_Different_Customer()
        {
            var cardValidator = new Mock<ICardValidator>();
            cardValidator.Setup(x => x.ValidateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new ValidateTokenResponse()));

            var cardRepository = new Mock<ICardRepository>();
            cardRepository.Setup(x => x.GetCardByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new CardEntity() { CustomerId = 2, CardNumber = 11111111234 }));
            cardRepository.Setup(x => x.GetRegistrationByCvvAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new TokenRegistrationEntity { CreationDate = DateTime.UtcNow.AddDays(-10) }));

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(x => x.ValidateTokenAsync(It.IsAny<CardEntity>(), It.IsAny<long>(), It.IsAny<int>())).Returns(Task.FromResult(true));
            tokenService.Setup(x => x.ValidateTokenExpirationAsync(It.IsAny<TokenRegistrationEntity>())).Returns(Task.FromResult(true));

            var cardService = new CardService(cardValidator.Object, tokenService.Object, cardRepository.Object,
                new Mock<IMapper>().Object, new Mock<ILogger<CardService>>().Object);

            var response = await cardService.ValidateTokenAsync(1, 1, (long)3412, 2);

            Assert.Null(response.Error);
            Assert.False(response.Validated);
        }

    }
}
