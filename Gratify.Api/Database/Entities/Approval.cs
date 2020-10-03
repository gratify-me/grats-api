using System;
using System.ComponentModel.DataAnnotations;
using Slack.Client.Chat;

namespace Gratify.Api.Database.Entities
{
    public class Approval : Entity
    {
        public Approval(Guid correlationId, DateTime approvedAt)
        {
            CorrelationId = correlationId;
            ApprovedAt = approvedAt;
        }

        [Required]
        public DateTime ApprovedAt { get; private set; }

        public string ReceiverNotificationChannel { get; set; }

        public string ReceiverNotificationTimestamp { get; set; }

        [Required]
        public int ReviewId { get; set; }

        [Required]
        public Review Review { get; set; }

        public ApiResponse ReceiverNotification => new ApiResponse
        {
            Channel = ReceiverNotificationChannel,
            Timestamp = ReceiverNotificationTimestamp,
        };

        public void SetReceiverNotification(ApiResponse receiverNotification)
        {
            ReceiverNotificationChannel = receiverNotification.Channel;
            ReceiverNotificationTimestamp = receiverNotification.Timestamp;
        }
    }
}
