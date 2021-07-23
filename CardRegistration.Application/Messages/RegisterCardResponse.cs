using System;

namespace CardRegistration.Application.Messages
{
    /// <summary>
    /// Registration response
    /// </summary>
    public class RegisterCardResponse : BaseResponse
    {
        /// <summary>
        /// Registration Date in UTC format
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Generated Token
        /// </summary>
        public long Token { get; set; }

        /// <summary>
        /// Card Id
        /// </summary>
        public int CardId { get; set; }
    }
}
