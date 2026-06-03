using System;
using System.Collections.Generic;
using Ex03.GarageLogic;

namespace Ex03.PaymentTests
{
    // Gateway whose Authorize throws, simulating a transport/network failure.
    // Used to assert the processor surfaces unexpected failures as an Error
    // (not a decline) rather than letting the exception escape.
    internal class ThrowingGateway : IPaymentGateway
    {
        public PaymentAuthorization Authorize(decimal amount, string cardNumber, string cardHolderName)
        {
            throw new InvalidOperationException("network down");
        }

        public PaymentCapture Capture(string authorizationId, decimal amount)
        {
            throw new InvalidOperationException("network down");
        }

        public PaymentRefund Refund(string transactionId, decimal amount)
        {
            throw new InvalidOperationException("network down");
        }

        public bool VoidAuthorization(string authorizationId)
        {
            throw new InvalidOperationException("network down");
        }
    }

    // Records the order of gateway calls so tests can prove the flow runs
    // Authorize -> Capture through the IPaymentGateway abstraction (not bypassed).
    internal class RecordingGateway : IPaymentGateway
    {
        public readonly List<string> Calls = new List<string>();

        public PaymentAuthorization Authorize(decimal amount, string cardNumber, string cardHolderName)
        {
            Calls.Add("Authorize");
            return new PaymentAuthorization { Success = true, AuthorizationId = "AUTH-1" };
        }

        public PaymentCapture Capture(string authorizationId, decimal amount)
        {
            Calls.Add("Capture:" + authorizationId);
            return new PaymentCapture { Success = true, TransactionId = "TXN-1", CapturedAmount = amount };
        }

        public PaymentRefund Refund(string transactionId, decimal amount)
        {
            Calls.Add("Refund");
            return new PaymentRefund { Success = true, RefundId = "REF-1", RefundedAmount = amount };
        }

        public bool VoidAuthorization(string authorizationId)
        {
            Calls.Add("Void");
            return true;
        }
    }

    internal static class TestData
    {
        public const string SuccessCard = "4242424242424242";
        public const string InsufficientFundsCard = "4111111111111111";
        public const string ExpiredCard = "4222000000000004";
        public const string DeclinedCard = "4333000000000000";
        public const string InvalidCard = "4000000000000001"; // fails Luhn
        public const string Holder = "Alice Driver";

        public static Garage GarageWithFixedCar(string i_License, string i_Phone = "0500000000", bool i_Returning = false)
        {
            Garage garage = new Garage();
            Vehicle car = VehicleCreator.Create(i_License, VehicleCreator.eVehicleType.FuelCar);
            garage.AddVehicle(car, "Alice", i_Phone, i_Returning);
            garage.UpdateVehicleStatus(i_License, Garage.eVehicleStatus.Fixed);

            return garage;
        }

        public static bool HasStatus(Garage i_Garage, string i_License, Garage.eVehicleStatus i_Status)
        {
            return i_Garage.GetLicenseListAccordingStatus(i_Status).ToString().Contains(i_License);
        }
    }
}
