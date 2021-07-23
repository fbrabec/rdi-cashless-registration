using System;
using System.Collections.Generic;

namespace CardRegistration.Domain.Entities
{
    public class CardEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public long CardNumber { get; set; }

        public virtual List<TokenRegistrationEntity> TokenRegistrations { get; set; }
    }
}
