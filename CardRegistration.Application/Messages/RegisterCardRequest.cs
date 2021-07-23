namespace CardRegistration.Application.Messages
{
    /// <summary>
    /// Customer card data
    /// </summary>
    public class RegisterCardRequest
    {
        /// <summary>
        /// Customer Id
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Card Number
        /// </summary>
        public long CardNumber { get; set; }

        /// <summary>
        /// CVV
        /// </summary>
        public int CVV { get; set; }
    }
}
