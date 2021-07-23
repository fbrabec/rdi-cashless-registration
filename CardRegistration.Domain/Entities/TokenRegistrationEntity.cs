using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardRegistration.Domain.Entities
{
    public class TokenRegistrationEntity
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public int CVV { get; set; }
        public DateTime CreationDate { get; set; }

        [ForeignKey("CardId")]
        public virtual CardEntity Card { get; set; }
    }
}
