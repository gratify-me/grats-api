using System;
using System.ComponentModel.DataAnnotations;

namespace Gratify.Api.Database.Entities
{
    public class Receival : Entity
    {
        public Receival(Guid correlationId, DateTime receivedAt, string receiverName, string receiverAccountNumber, int amountReceived)
        {
            CorrelationId = correlationId;
            ReceivedAt = receivedAt;
            ReceiverName = receiverName;
            ReceiverAccountNumber = receiverAccountNumber;
            AmountReceived = amountReceived;
        }

        [Required]
        public DateTime ReceivedAt { get; private set; }

        [Required]
        public string ReceiverName { get; set; }

        // TODO: Storing account number requires some privacy adjustments
        // https://www.datatilsynet.no/personvern-pa-ulike-omrader/kundehandtering-handel-og-medlemskap/nettbutikker-og-kundeopplysninger/
        [Required]
        public string ReceiverAccountNumber { get; set; }

        [Required]
        public int AmountReceived { get; set; }

        public bool CreditTransferInitiated { get; set; }

        [Required]
        public int ApprovalId { get; set; }

        [Required]
        public Approval Approval { get; set; }
    }
}
