using CardRegistration.Application.Messages;
using CardRegistration.Application.Validators;
using CardRegistration.Infrastructure.Common.Constants;
using System;
using System.Threading.Tasks;

namespace CardRegistration.Application.Impl.Validators
{
    public class CardValidator : ICardValidator
    {
        public async Task<RegisterCardResponse> ValidateAsync(RegisterCardRequest request)
        {
            var response = new RegisterCardResponse();

            if (request == null)
            {
                response.AddError(Message.Code.CARD_E001.ToString(), Message.Code.CARD_E001.GetDescriptionEnum());
                return response;
            }

            if (request.CustomerId <= default(int))
            {
                response.AddError(Message.Code.CARD_V003.ToString(), Message.Code.CARD_V003.GetDescriptionEnum());
                return response;
            }
            
            if (request.CardNumber <= default(long))
            {
                response.AddError(Message.Code.CARD_V004.ToString(), Message.Code.CARD_V004.GetDescriptionEnum());
                return response;
            }

            if (request.CVV <= default(int))
            {
                response.AddError(Message.Code.CARD_V005.ToString(), Message.Code.CARD_V005.GetDescriptionEnum());
                return response;
            }

            if (request.CardNumber.ToString().Length > 16)
            {
                response.AddError(Message.Code.CARD_V001.ToString(), Message.Code.CARD_V001.GetDescriptionEnum());
                return response;
            }

            if (request.CVV.ToString().Length > 5)
            {
                response.AddError(Message.Code.CARD_V002.ToString(), Message.Code.CARD_V002.GetDescriptionEnum());
                return response;
            }

            if (request.CardNumber.ToString().Length < 4)
            {
                response.AddError(Message.Code.CARD_V013.ToString(), Message.Code.CARD_V013.GetDescriptionEnum());
                return response;
            }

            return response;
        }

        public async Task<ValidateTokenResponse> ValidateAsync(int cardId, int customerId, long token, int CVV)
        {
            var response = new ValidateTokenResponse();

            if (cardId <= default(int))
            {
                response.AddError(Message.Code.CARD_V006.ToString(), Message.Code.CARD_V006.GetDescriptionEnum());
                return response;
            }

            if (customerId <= default(int))
            {
                response.AddError(Message.Code.CARD_V003.ToString(), Message.Code.CARD_V003.GetDescriptionEnum());
                return response;
            }

            if (token <= default(int))
            {
                response.AddError(Message.Code.CARD_V007.ToString(), Message.Code.CARD_V007.GetDescriptionEnum());
                return response;
            }

            if (CVV <= default(int))
            {
                response.AddError(Message.Code.CARD_V005.ToString(), Message.Code.CARD_V005.GetDescriptionEnum());
                return response;
            }

            if (CVV.ToString().Length > 5)
            {
                response.AddError(Message.Code.CARD_V002.ToString(), Message.Code.CARD_V002.GetDescriptionEnum());
                return response;
            }

            return response;
        }
    }
}
