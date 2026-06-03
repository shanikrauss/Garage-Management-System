using System;
using System.Collections.Generic;
using System.Threading;

namespace Ex03.GarageLogic
{
    // Mock implementation of Visa payment gateway for testing
    // Simulates real Visa API behavior without external dependencies
    public class MockVisaGateway : IPaymentGateway
    {
        private readonly Dictionary<string, AuthorizationRecord> r_Authorizations;
        private readonly Dictionary<string, CaptureRecord> r_Captures;
        private readonly object r_LockObject = new object();
        private int r_TransactionCounter = 1000;

        private class AuthorizationRecord
        {
            public string AuthorizationId { get; set; }
            public decimal Amount { get; set; }
            public string CardNumber { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsVoided { get; set; }
            public bool IsCaptured { get; set; }
        }

        private class CaptureRecord
        {
            public string TransactionId { get; set; }
            public string AuthorizationId { get; set; }
            public decimal Amount { get; set; }
            public DateTime CapturedAt { get; set; }
            public bool IsRefunded { get; set; }
        }

        public MockVisaGateway()
        {
            r_Authorizations = new Dictionary<string, AuthorizationRecord>();
            r_Captures = new Dictionary<string, CaptureRecord>();
        }

        public PaymentAuthorization Authorize(decimal amount, string cardNumber, string cardHolderName)
        {
            // Simulate network delay
            Thread.Sleep(50);

            var result = new PaymentAuthorization();

            // Validate amount
            if (amount <= 0)
            {
                result.Success = false;
                result.ErrorCode = "INVALID_AMOUNT";
                result.ErrorMessage = "Amount must be greater than zero";
                return result;
            }

            if (amount > 5000)
            {
                result.Success = false;
                result.ErrorCode = "AMOUNT_TOO_HIGH";
                result.ErrorMessage = "Amount exceeds maximum allowed ($5000)";
                return result;
            }

            // Validate card number using Luhn algorithm
            if (!IsValidCardNumber(cardNumber))
            {
                result.Success = false;
                result.ErrorCode = "INVALID_CARD";
                result.ErrorMessage = "Card number is invalid";
                return result;
            }

            // Simulate card-specific responses
            if (cardNumber.StartsWith("4111"))
            {
                result.Success = false;
                result.ErrorCode = "INSUFFICIENT_FUNDS";
                result.ErrorMessage = "Card has insufficient funds";
                return result;
            }

            if (cardNumber.StartsWith("4222"))
            {
                result.Success = false;
                result.ErrorCode = "CARD_EXPIRED";
                result.ErrorMessage = "Card has expired";
                return result;
            }

            if (cardNumber.StartsWith("4333"))
            {
                result.Success = false;
                result.ErrorCode = "CARD_DECLINED";
                result.ErrorMessage = "Card was declined by issuer";
                return result;
            }

            // Success case - card starting with 4000 or other valid numbers
            lock (r_LockObject)
            {
                string authId = $"VISA_AUTH_{r_TransactionCounter++}_{DateTime.Now.Ticks}";
                r_Authorizations[authId] = new AuthorizationRecord
                {
                    AuthorizationId = authId,
                    Amount = amount,
                    CardNumber = cardNumber,
                    CreatedAt = DateTime.Now,
                    IsVoided = false,
                    IsCaptured = false
                };

                result.Success = true;
                result.AuthorizationId = authId;
            }

            return result;
        }

        public PaymentCapture Capture(string authorizationId, decimal amount)
        {
            // Simulate network delay
            Thread.Sleep(50);

            var result = new PaymentCapture();

            lock (r_LockObject)
            {
                if (!r_Authorizations.ContainsKey(authorizationId))
                {
                    result.Success = false;
                    result.ErrorCode = "INVALID_AUTH";
                    result.ErrorMessage = "Authorization ID not found";
                    return result;
                }

                var auth = r_Authorizations[authorizationId];

                if (auth.IsVoided)
                {
                    result.Success = false;
                    result.ErrorCode = "AUTH_VOIDED";
                    result.ErrorMessage = "Authorization has been voided";
                    return result;
                }

                if (auth.IsCaptured)
                {
                    result.Success = false;
                    result.ErrorCode = "ALREADY_CAPTURED";
                    result.ErrorMessage = "Authorization has already been captured";
                    return result;
                }

                if (amount > auth.Amount)
                {
                    result.Success = false;
                    result.ErrorCode = "AMOUNT_EXCEEDS_AUTH";
                    result.ErrorMessage = "Capture amount exceeds authorized amount";
                    return result;
                }

                // Success
                string transactionId = $"VISA_TXN_{r_TransactionCounter++}_{DateTime.Now.Ticks}";
                r_Captures[transactionId] = new CaptureRecord
                {
                    TransactionId = transactionId,
                    AuthorizationId = authorizationId,
                    Amount = amount,
                    CapturedAt = DateTime.Now,
                    IsRefunded = false
                };

                auth.IsCaptured = true;

                result.Success = true;
                result.TransactionId = transactionId;
                result.CapturedAmount = amount;
            }

            return result;
        }

        public PaymentRefund Refund(string transactionId, decimal amount)
        {
            // Simulate network delay
            Thread.Sleep(50);

            var result = new PaymentRefund();

            lock (r_LockObject)
            {
                if (!r_Captures.ContainsKey(transactionId))
                {
                    result.Success = false;
                    result.ErrorCode = "INVALID_TXN";
                    result.ErrorMessage = "Transaction ID not found";
                    return result;
                }

                var capture = r_Captures[transactionId];

                if (capture.IsRefunded)
                {
                    result.Success = false;
                    result.ErrorCode = "ALREADY_REFUNDED";
                    result.ErrorMessage = "Transaction has already been refunded";
                    return result;
                }

                if (amount > capture.Amount)
                {
                    result.Success = false;
                    result.ErrorCode = "REFUND_EXCEEDS_CAPTURE";
                    result.ErrorMessage = "Refund amount exceeds captured amount";
                    return result;
                }

                // Success
                capture.IsRefunded = true;
                result.Success = true;
                result.RefundId = $"VISA_REF_{r_TransactionCounter++}_{DateTime.Now.Ticks}";
                result.RefundedAmount = amount;
            }

            return result;
        }

        public bool VoidAuthorization(string authorizationId)
        {
            // Simulate network delay
            Thread.Sleep(30);

            lock (r_LockObject)
            {
                if (!r_Authorizations.ContainsKey(authorizationId))
                {
                    return false;
                }

                var auth = r_Authorizations[authorizationId];

                if (auth.IsCaptured || auth.IsVoided)
                {
                    return false;
                }

                auth.IsVoided = true;
                return true;
            }
        }

        // Luhn algorithm for card number validation
        private bool IsValidCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            // Remove spaces and dashes
            cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

            if (cardNumber.Length < 13 || cardNumber.Length > 19)
                return false;

            foreach (char c in cardNumber)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            // Luhn algorithm
            int sum = 0;
            bool alternate = false;
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int digit = cardNumber[i] - '0';
                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }
                sum += digit;
                alternate = !alternate;
            }

            return sum % 10 == 0;
        }
    }
}
