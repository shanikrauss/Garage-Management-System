using System;

namespace Ex03.GarageLogic
{
    // Interface for payment gateway abstraction
    // Allows integration with external payment processors (Visa, Mastercard, etc.)
    public interface IPaymentGateway
    {
        // Authorize a payment (hold funds without capturing)
        // Returns authorization ID if successful
        PaymentAuthorization Authorize(decimal amount, string cardNumber, string cardHolderName);
        
        // Capture a previously authorized payment (transfer funds)
        PaymentCapture Capture(string authorizationId, decimal amount);
        
        // Refund a captured payment
        PaymentRefund Refund(string transactionId, decimal amount);
        
        // Void an authorization (release held funds)
        bool VoidAuthorization(string authorizationId);
    }

    public class PaymentAuthorization
    {
        public bool Success { get; set; }
        public string AuthorizationId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }

        public PaymentAuthorization()
        {
            CreatedAt = DateTime.Now;
        }
    }

    public class PaymentCapture
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public decimal CapturedAmount { get; set; }
        public DateTime CapturedAt { get; set; }

        public PaymentCapture()
        {
            CapturedAt = DateTime.Now;
        }
    }

    public class PaymentRefund
    {
        public bool Success { get; set; }
        public string RefundId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public decimal RefundedAmount { get; set; }
        public DateTime RefundedAt { get; set; }

        public PaymentRefund()
        {
            RefundedAt = DateTime.Now;
        }
    }

    public enum PaymentStatus
    {
        Pending,
        Authorized,
        Captured,
        Failed,
        Refunded,
        Voided
    }
}
