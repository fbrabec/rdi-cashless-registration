using Microsoft.AspNetCore.Mvc;
using CardRegistration.Application.Messages;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using CardRegistration.Application.Services;
using CardRegistration.Infrastructure.Common.Constants;

namespace CardRegistration.Host.WebApi.Controllers
{
    [ApiController]
    [Route("cards")]
    [Produces(MediaTypeNames.Application.Json)]
    public class CardController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ICardService _cardService;

        public CardController(ILogger<CardController> logger,
                              IMapper mapper,
                              ICardService cardService)
        {
            _logger = logger;
            _mapper = mapper;
            _cardService = cardService;
        }

        /// <summary>
        /// Register a customer card and generates a token
        /// </summary>
        /// <param name="request">Customer card data</param>
        /// <returns>Token generated</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RegisterCardResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Register(RegisterCardRequest request)
        {
            var result = await _cardService.RegisterCardAsync(request);

            if (result.Error != null)
            {
                if (!result.Error.Code.Equals(Message.Code.CARD_E002.ToString()))
                    return BadRequest(result);
                else
                    return StatusCode((int)HttpStatusCode.InternalServerError, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Validate token and customer data
        /// </summary>
        /// <param name="cardId">Card Id</param>
        /// <param name="customerId">Customer Id</param>
        /// <param name="cvv">CVV</param>
        /// <param name="token">Token</param>
        /// <returns>Boolean indicating whether the token is valid or not</returns>
        [HttpGet]
        [Route("{cardId}/token/validate")]
        [ProducesResponseType(typeof(ValidateTokenResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ValidateToken([FromRoute] int cardId, [FromQuery] int customerId, [FromQuery] long token, [FromQuery] int cvv)
        {
            var result = await _cardService.ValidateTokenAsync(cardId, customerId, token, cvv);

            if (result.Error != null)
            {
                if (!result.Error.Code.Equals(Message.Code.CARD_E002.ToString()))
                    return BadRequest(result);
                else
                    return StatusCode((int)HttpStatusCode.InternalServerError, result);
            }

            return Ok(result);
        }
    }
}
