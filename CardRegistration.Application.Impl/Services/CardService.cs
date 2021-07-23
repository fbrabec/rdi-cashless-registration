using AutoMapper;
using CardRegistration.Application.Messages;
using CardRegistration.Application.Services;
using CardRegistration.Application.Validators;
using CardRegistration.Domain.Contracts.Repositories;
using CardRegistration.Domain.Contracts.Services;
using CardRegistration.Domain.Entities;
using CardRegistration.Infrastructure.Common.Constants;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CardRegistration.Application.Impl.Services
{
    public class CardService : ICardService
    {
        private readonly ICardValidator _validator;
        private readonly ITokenService _tokenService;
        private readonly ICardRepository _cardRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CardService(ICardValidator validator,
                           ITokenService tokenService,
                           ICardRepository cardRepository,
                           IMapper mapper,
                           ILogger<CardService> logger)
        {
            _validator = validator;
            _tokenService = tokenService;
            _cardRepository = cardRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RegisterCardResponse> RegisterCardAsync(RegisterCardRequest request)
        {
            try
            {
                // Validate input
                var response = await _validator.ValidateAsync(request);
                
                if (response.Error != null)
                    return response;

                var cardEntity = _mapper.Map<CardEntity>(request);

                // Generate the token
                var token = await _tokenService.GenerateTokenAsync(cardEntity, request.CVV);

                // Get card by number to check if it already exists
                var existingCard = await _cardRepository.GetCardByNumberAsync(request.CardNumber);

                int cardId;

                // If card exists, check if the token has already been registered.
                // If so, return validation message, informing the CVV has already been used.
                if (existingCard != null)
                {
                    var existingToken = await _cardRepository.GetRegistrationByCvvAsync(existingCard.Id, request.CVV);

                    response.CardId = existingCard.Id;
                    response.AddError(Message.Code.CARD_V010.ToString(), Message.Code.CARD_V010.GetDescriptionEnum());

                    return response;
                }

                // Add card to the database
                if (existingCard == null)
                    cardId = await _cardRepository.AddCardAsync(cardEntity);
                else
                    cardId = existingCard.Id;

                var tokenRegistration = new TokenRegistrationEntity()
                {
                    CardId = cardId,
                    CVV = request.CVV,
                    CreationDate = DateTime.UtcNow
                };

                // Add registration history to the database
                await _cardRepository.AddTokenRegistrationAsync(tokenRegistration);

                response.CardId = cardId;
                response.RegistrationDate = tokenRegistration.CreationDate;
                response.Token = token;

                return response;
            }
            catch (Exception ex)
            {
                var response = new RegisterCardResponse();
                response.AddError(Message.Code.CARD_E002.ToString(), ex.Message);

                return response;
            }
        }

        public async Task<ValidateTokenResponse> ValidateTokenAsync(int cardId, int customerId, long token, int cvv)
        {
            try
            {
                // Validate input
                var response = await _validator.ValidateAsync(cardId, customerId, token, cvv);

                if (response.Error != null)
                    return response;

                var card = await _cardRepository.GetCardByIdAsync(cardId);

                if (card == null)
                {
                    response.AddError(Message.Code.CARD_V014.ToString(), Message.Code.CARD_V014.GetDescriptionEnum());

                    return response;
                }

                _logger.LogInformation($"Card Number: {card.CardNumber}");

                var registration = await _cardRepository.GetRegistrationByCvvAsync(card.Id, cvv);

                if (registration == null)
                {
                    response.AddError(Message.Code.CARD_V012.ToString(), $"Could not find any registration with the CVV number informed: {cvv}");

                    return response;
                }

                if (card.CustomerId != customerId)
                {
                    response.Validated = false;
                    return response;
                }

                var tokenIsEqual = await _tokenService.ValidateTokenAsync(card, token, cvv);

                var tokenIsValid = await _tokenService.ValidateTokenExpirationAsync(registration);

                response.Validated = tokenIsEqual && tokenIsValid;

                return response;
            }
            catch (Exception ex)
            {
                var response = new ValidateTokenResponse();
                response.AddError(Message.Code.CARD_E002.ToString(), ex.Message);

                return response;
            }
        }
    }
}
