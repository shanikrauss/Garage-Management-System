using System;
using Ex03.GarageLogic;
using Xunit;

namespace Ex03.PaymentTests
{
    // The Authorize -> Capture -> status-update flow, validation, and gateway
    // response handling (success / declined / error).
    public class PaymentFlowTests
    {
        private static readonly MockVisaGateway s_Gateway = new MockVisaGateway();

        [Fact]
        public void FullPayment_OnSuccessfulCapture_TransitionsToPaid()
        {
            Garage garage = TestData.GarageWithFixedCar("C1");
            decimal total = garage.GetBalanceDue("C1"); // 108.50

            PaymentResult result = garage.ProcessPayment("C1", TestData.SuccessCard, TestData.Holder, total, s_Gateway);

            Assert.True(result.Success);
            Assert.Equal(ePaymentResultStatus.Success, result.Status);
            Assert.Equal(0m, result.BalanceDue);
            Assert.False(string.IsNullOrEmpty(result.TransactionId));
            Assert.True(TestData.HasStatus(garage, "C1", Garage.eVehicleStatus.Paid));
        }

        [Fact]
        public void Payment_RejectedWhenVehicleNotFixed()
        {
            Garage garage = new Garage();
            Vehicle car = VehicleCreator.Create("C1", VehicleCreator.eVehicleType.FuelCar);
            garage.AddVehicle(car, "Alice", "0500000000"); // stays InProgress

            PaymentResult result = garage.ProcessPayment("C1", TestData.SuccessCard, TestData.Holder, 50m, s_Gateway);

            Assert.False(result.Success);
            Assert.Equal(ePaymentResultStatus.ValidationFailed, result.Status);
            Assert.Equal("INVALID_STATUS", result.ErrorCode);
            Assert.False(TestData.HasStatus(garage, "C1", Garage.eVehicleStatus.Paid));
        }

        [Fact]
        public void Payment_RejectedForNonPositiveAmount()
        {
            Garage garage = TestData.GarageWithFixedCar("C1");

            PaymentResult result = garage.ProcessPayment("C1", TestData.SuccessCard, TestData.Holder, 0m, s_Gateway);

            Assert.False(result.Success);
            Assert.Equal(ePaymentResultStatus.ValidationFailed, result.Status);
            Assert.Equal("INVALID_AMOUNT", result.ErrorCode);
        }

        [Fact]
        public void Payment_RejectedWhenAmountExceedsBalance()
        {
            Garage garage = TestData.GarageWithFixedCar("C1"); // balance 108.50

            PaymentResult result = garage.ProcessPayment("C1", TestData.SuccessCard, TestData.Holder, 200m, s_Gateway);

            Assert.False(result.Success);
            Assert.Equal(ePaymentResultStatus.ValidationFailed, result.Status);
            Assert.Equal("AMOUNT_EXCEEDS_BALANCE", result.ErrorCode);
        }

        [Fact]
        public void Payment_RejectedForUnknownVehicle()
        {
            Garage garage = new Garage();

            PaymentResult result = garage.ProcessPayment("NOPE", TestData.SuccessCard, TestData.Holder, 50m, s_Gateway);

            Assert.False(result.Success);
            Assert.Equal(ePaymentResultStatus.ValidationFailed, result.Status);
            Assert.Equal("VEHICLE_NOT_FOUND", result.ErrorCode);
        }

        [Fact]
        public void InsufficientFundsCard_IsDeclined_AndStatusUnchanged()
        {
            Garage garage = TestData.GarageWithFixedCar("C1");
            decimal balanceBefore = garage.GetBalanceDue("C1");

            PaymentResult result = garage.ProcessPayment("C1", TestData.InsufficientFundsCard, TestData.Holder, balanceBefore, s_Gateway);

            Assert.False(result.Success);
            Assert.Equal(ePaymentResultStatus.Declined, result.Status);
            Assert.Equal("INSUFFICIENT_FUNDS", result.ErrorCode);
            Assert.False(TestData.HasStatus(garage, "C1", Garage.eVehicleStatus.Paid));
            Assert.True(TestData.HasStatus(garage, "C1", Garage.eVehicleStatus.Fixed));
            Assert.Equal(balanceBefore, garage.GetBalanceDue("C1")); // unchanged
        }

        [Fact]
        public void ExpiredCard_IsDeclined()
        {
            Garage garage = TestData.GarageWithFixedCar("C1");

            PaymentResult result = garage.ProcessPayment("C1", TestData.ExpiredCard, TestData.Holder, garage.GetBalanceDue("C1"), s_Gateway);

            Assert.False(result.Success);
            Assert.Equal(ePaymentResultStatus.Declined, result.Status);
            Assert.Equal("CARD_EXPIRED", result.ErrorCode);
        }

        [Fact]
        public void IssuerDeclinedCard_IsDeclined()
        {
            Garage garage = TestData.GarageWithFixedCar("C1");

            PaymentResult result = garage.ProcessPayment("C1", TestData.DeclinedCard, TestData.Holder, garage.GetBalanceDue("C1"), s_Gateway);

            Assert.False(result.Success);
            Assert.Equal(ePaymentResultStatus.Declined, result.Status);
            Assert.Equal("CARD_DECLINED", result.ErrorCode);
        }

        [Fact]
        public void InvalidCard_IsTreatedAsError_NotDecline()
        {
            Garage garage = TestData.GarageWithFixedCar("C1");

            PaymentResult result = garage.ProcessPayment("C1", TestData.InvalidCard, TestData.Holder, garage.GetBalanceDue("C1"), s_Gateway);

            Assert.False(result.Success);
            Assert.Equal(ePaymentResultStatus.Error, result.Status);
            Assert.Equal("INVALID_CARD", result.ErrorCode);
        }

        [Fact]
        public void GatewayException_IsTreatedAsError_NotDecline()
        {
            Garage garage = TestData.GarageWithFixedCar("C1");

            PaymentResult result = garage.ProcessPayment("C1", TestData.SuccessCard, TestData.Holder, 50m, new ThrowingGateway());

            Assert.False(result.Success);
            Assert.Equal(ePaymentResultStatus.Error, result.Status);
            Assert.Equal("GATEWAY_ERROR", result.ErrorCode);
            Assert.False(TestData.HasStatus(garage, "C1", Garage.eVehicleStatus.Paid));
        }
    }
}
