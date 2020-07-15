using System;
using System.ComponentModel.DataAnnotations;
using Slack.Client.Chat;

namespace Gratify.Api.Database.Entities
{
    public class Review : Entity
    {
        public Review()
        { }

        public Review(Guid correlationId, DateTime requestedAt, string reviewer, ApiResponse authorNotification)
        {
            CorrelationId = correlationId;
            RequestedAt = requestedAt;
            Reviewer = reviewer;
            AuthorNotificationChannel = authorNotification.Channel;
            AuthorNotificationTimestamp = authorNotification.Timestamp;
            IsForwarded = false;
        }

        [Required]
        public DateTime RequestedAt { get; private set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string Reviewer { get; private set; }

        public int? ForwardedFrom { get; private set; }

        [Required]
        public bool IsForwarded { get; private set; }

        [Required]
        public bool IsNotified { get; set; }

        [Required]
        public int GratsId { get; set; }

        [Required]
        public Grats Grats { get; set; }

        [Required]
        public string AuthorNotificationChannel { get; set; }

        [Required]
        public string AuthorNotificationTimestamp { get; set; }

        [Required]
        public string ReviewRequestChannel { get; set; }

        [Required]
        public string ReviewRequestTimestamp { get; set; }

        public Approval Approval { get; set; }

        public Denial Denial { get; set; }

        public Review ForwardTo(string newReviewerId)
        {
            IsForwarded = true;

            return new Review
            {
                CorrelationId = CorrelationId,
                RequestedAt = DateTime.UtcNow,
                Reviewer = newReviewerId,
                Grats = Grats,
                AuthorNotificationChannel = AuthorNotificationChannel,
                AuthorNotificationTimestamp = AuthorNotificationTimestamp,
                TeamId = TeamId,
                ForwardedFrom = Id
            };
        }

        public ApiResponse AuthorNotification => new ApiResponse
        {
            Channel = AuthorNotificationChannel,
            Timestamp = AuthorNotificationTimestamp,
        };

        public ApiResponse ReviewRequest => new ApiResponse
        {
            Channel = ReviewRequestChannel,
            Timestamp = ReviewRequestTimestamp,
        };

        public void SetReviewRequest(ApiResponse reviewRequest)
        {
            ReviewRequestChannel = reviewRequest.Channel;
            ReviewRequestTimestamp = reviewRequest.Timestamp;
        }
    }
}
