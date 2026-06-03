using System;
using System.Collections.Generic;
using System.Threading;

namespace Ex03.GarageLogic
{
    // Mock implementation of Mastercard payment gateway
    // Different behavior from Visa to demonstrate gateway abstraction
    public class MockMastercardGateway : IPaymentGateway
    {
        private readonly Dictionary<string, AuthorizationRecord> r_Authorizations;
        private readonly Dictionary<string, CaptureRecord> r_Captures;
        private readonly object r_LockObject = new object();
        private int r_TransactionCounter = 2000;

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

        public MockMastercardGateway()
        {
            r_Authorizations = new Dictionary<string, AuthorizationRecord>();
            r_Captures = new Dictionary<string, CaptureRecord>();
        }

        public PaymentAuthorization Authorize(decimal amount, string cardNumber, string cardHolderName)
        {
            // Mastercard has slightly faster response time
            Thread.Sleep(30);

            var result = new PaymentAuthorization();

            // Validate amount - Mastercard has higher limit
            if (amount <= 0)
            {
                result.Success = false;
                result.ErrorCode = "INVALID_AMOUNT";
                result.ErrorMessage = "Amount must be greater than zero";
                return result;
            }

            if (amount > 10000)
            {
                result.Success = false;
                result.ErrorCode = "AMOUNT_TOO_HIGH";
                result.ErrorMessage = "Amount exceeds maximum allowed ($10000)";
                return result;
            }

            // Validate card number
            if (!IsValidCardNumber(cardNumber))
            {
                result.Success = false;
                result.ErrorCode = "INVALID_CARD";
                result.ErrorMessage = "Card number is invalid";
                return result;
            }

            // Mastercard-specific test cards
            // 5555 series - success
            // 5105 series - insufficient funds
            // 5204 series - expired
            // 5300 series - suspected fraud
            if (cardNumber.StartsWith("5105"))
            {
                result.Success = false;
                result.ErrorCode = "INSUFFICIENT_FUNDS";
                result.ErrorMessage = "Card has insufficient funds";
                return result;
            }

            if (cardNumber.StartsWith("5204"))
            {
                result.Success = false;
                result.ErrorCode = "CARD_EXPIRED";
                result.ErrorMessage = "Card has expired";
                return result;
            }

            if (cardNumber.StartsWith("5300"))
            {
                result.Success = false;
                result.ErrorCode = "SUSPECTED_FRAUD";
                result.ErrorMessage = "Transaction flagged for suspected fraud";
                return result;
            }

            // Success case
            lock (r_LockObject)
            {
                string authId = $"MC_AUTH_{r_TransactionCounter++}_{DateTime.Now.Ticks}";
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
            Thread.Sleep(30);

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

                // Mastercard allows partial capture up to 115% (tip adjustment)
                if (amount > auth.Amount * 1.15m)
                {
                    result.Success = false;
                    result.ErrorCode = "AMOUNT_EXCEEDS_LIMIT";
                    result.ErrorMessage = "Capture amount exceeds 115% of authorized amount";
                    return result;
                }

                string transactionId = $"MC_TXN_{r_TransactionCounter++}_{DateTime.Now.Ticks}";
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
            Thread.Sleep(30);

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

                capture.IsRefunded = true;
                result.Success = true;
                result.RefundId = $"MC_REF_{r_TransactionCounter++}_{DateTime.Now.Ticks}";
                result.RefundedAmount = amount;
            }

            return result;
        }

        public bool VoidAuthorization(string authorizationId)
        {
            Thread.Sleep(20);

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

        private bool IsValidCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

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
