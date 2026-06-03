using System;
using Ex03.GarageLogic;
using Xunit;

namespace Ex03.PaymentTests
{
    // Direct tests of PaymentProcessor: it drives Authorize -> Capture through the
    // IPaymentGateway abstraction and maps responses without bypassing the interface.
    public class PaymentProcessorTests
    {
        [Fact]
        public void ProcessPayment_RunsAuthorizeThenCapture_InOrder()
        {
            RecordingGateway gateway = new RecordingGateway();
            PaymentProcessor processor = new PaymentProcessor(gateway);

            PaymentResult result = processor.ProcessPayment(120m, TestData.SuccessCard, TestData.Holder);

            Assert.True(result.Success);
            Assert.Equal(2, gateway.Calls.Count);
            Assert.Equal("Authorize", gateway.Calls[0]);
            Assert.StartsWith("Capture:AUTH-1", gateway.Calls[1]);
        }

        [Fact]
        public void ProcessPayment_SuccessfulCapture_ReturnsTransactionAndAmount()
        {
            PaymentProcessor processor = new PaymentProcessor(new MockVisaGateway());

            PaymentResult result = processor.ProcessPayment(120m, TestData.SuccessCard, TestData.Holder);

            Assert.True(result.Success);
            Assert.Equal(ePaymentResultStatus.Success, result.Status);
            Assert.Equal(120m, result.AmountPaid);
            Assert.False(string.IsNullOrEmpty(result.TransactionId));
        }

        [Fact]
        public void ProcessPayment_NonPositiveAmount_FailsValidationWithoutCallingGateway()
        {
            RecordingGateway gateway = new RecordingGateway();
            PaymentProcessor processor = new PaymentProcessor(gateway);

            PaymentResult result = processor.ProcessPayment(0m, TestData.SuccessCard, TestData.Holder);

            Assert.False(result.Success);
            Assert.Equal(ePaymentResultStatus.ValidationFailed, result.Status);
            Assert.Empty(gateway.Calls);
        }

        [Fact]
        public void Constructor_NullGateway_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PaymentProcessor(null));
        }
    }
}
