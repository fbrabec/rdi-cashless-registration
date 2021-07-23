using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CardRegistration.Infrastructure.Common.Constants
{
    public static class Message
    {
        public enum Code
        {
            /// <summary>
            /// Request is null or empty
            /// </summary>
            [Description("Request is null or empty")]
            CARD_E001,
            /// <summary>
            /// Internal error
            /// </summary>
            [Description("Internal error")]
            CARD_E002,
            /// <summary>
            /// Card Number exceeded the limit of 16 characters
            /// </summary>
            [Description("Card Number exceeded the limit of 16 characters")]
            CARD_V001,
            /// <summary>
            /// CVV exceeded the limit of 5 characters
            /// </summary>
            [Description("CVV exceeded the limit of 5 characters")]
            CARD_V002,
            /// <summary>
            /// CustomerId is required
            /// </summary>
            [Description("CustomerId is required")]
            CARD_V003,
            /// <summary>
            /// Card Number is required
            /// </summary>
            [Description("Card Number is required")]
            CARD_V004,
            /// <summary>
            /// CVV is required
            /// </summary>
            [Description("CVV is required")]
            CARD_V005,
            /// <summary>
            /// CardId is required
            /// </summary>
            [Description("CardId is required")]
            CARD_V006,
            /// <summary>
            /// Token is required
            /// </summary>
            [Description("Token is required")]
            CARD_V007,
            /// <summary>
            /// Token expired
            /// </summary>
            [Description("Token expired")]
            CARD_V008,
            /// <summary>
            /// Customer informed is different from the customer registered in the card
            /// </summary>
            [Description("Customer informed is different from the customer registered in the card")]
            CARD_V009,
            /// <summary>
            /// Token already registered
            /// </summary>
            [Description("Token already registered")]
            CARD_V010,
            /// <summary>
            /// Invalid token
            /// </summary>
            [Description("Invalid token")]
            CARD_V011,
            /// <summary>
            /// Could not find any registration with the CVV number informed
            /// </summary>
            [Description("Could not find any registration with the CVV number informed")]
            CARD_V012,
            /// <summary>
            /// Card Number must have at least 4 characters
            /// </summary>
            [Description("Card Number must have at least 4 characters")]
            CARD_V013,
            /// <summary>
            /// Card not found
            /// </summary>
            [Description("Card not found")]
            CARD_V014
        }

        public static string GetDescriptionEnum(this Code value)
        {
            var enumMember = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var descriptionAttribute =
                enumMember == null
                    ? default(DescriptionAttribute)
                    : enumMember.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return
                descriptionAttribute == null
                    ? value.ToString()
                    : descriptionAttribute.Description;
        }
    }
}
