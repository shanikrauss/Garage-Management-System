using System;
using Ex03.GarageLogic;
using Xunit;

namespace Ex03.PaymentTests
{
    // Payment plans: partial payments, balance tracking, and multiple payments
    // per vehicle until the balance is cleared.
    public class PaymentPlanTests
    {
        private static readonly MockVisaGateway s_Gateway = new MockVisaGateway();

        [Fact]
        public void PartialPayment_ReducesBalance_AndStaysFixed()
        {
            Garage garage = TestData.GarageWithFixedCar("C1"); // total 108.50

            PaymentResult result = garage.ProcessPayment("C1", TestData.SuccessCard, TestData.Holder, 50m, s_Gateway);

            Assert.True(result.Success);
            Assert.Equal(ePaymentResultStatus.PartialPayment, result.Status);
            Assert.Equal(58.50m, result.BalanceDue);
            Assert.Equal(58.50m, garage.GetBalanceDue("C1"));
            Assert.True(TestData.HasStatus(garage, "C1", Garage.eVehicleStatus.Fixed));
            Assert.False(TestData.HasStatus(garage, "C1", Garage.eVehicleStatus.Paid));
        }

        [Fact]
        public void MultiplePartialPayments_ClearBalance_AndMarkPaid()
        {
            Garage garage = TestData.GarageWithFixedCar("C1"); // total 108.50

            PaymentResult first = garage.ProcessPayment("C1", TestData.SuccessCard, TestData.Holder, 100m, s_Gateway);
            Assert.Equal(ePaymentResultStatus.PartialPayment, first.Status);
            Assert.Equal(8.50m, garage.GetBalanceDue("C1"));

            PaymentResult second = garage.ProcessPayment("C1", TestData.SuccessCard, TestData.Holder, 8.50m, s_Gateway);

            Assert.True(second.Success);
            Assert.Equal(ePaymentResultStatus.Success, second.Status);
            Assert.Equal(0m, garage.GetBalanceDue("C1"));
            Assert.True(TestData.HasStatus(garage, "C1", Garage.eVehicleStatus.Paid));
        }

        [Fact]
        public void DeclinedPartialPayment_DoesNotReduceBalance()
        {
            Garage garage = TestData.GarageWithFixedCar("C1");
            decimal before = garage.GetBalanceDue("C1");

            PaymentResult result = garage.ProcessPayment("C1", TestData.InsufficientFundsCard, TestData.Holder, 50m, s_Gateway);

            Assert.False(result.Success);
            Assert.Equal(before, garage.GetBalanceDue("C1"));
        }
    }
}
