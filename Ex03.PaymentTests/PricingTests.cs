using System;
using Ex03.GarageLogic;
using Xunit;

namespace Ex03.PaymentTests
{
    // Dynamic-pricing rules: base rate by vehicle type, $50/hr labor, parts,
    // 8.5% tax, and the returning-customer / fleet discounts.
    public class PricingTests
    {
        [Fact]
        public void BaseRate_Car_Is100()
        {
            Garage garage = new Garage();
            Vehicle car = VehicleCreator.Create("C1", VehicleCreator.eVehicleType.FuelCar);

            Assert.Equal(100m, garage.CalculateBaseRate(car));
        }

        [Fact]
        public void BaseRate_Truck_Is200()
        {
            Garage garage = new Garage();
            Vehicle truck = VehicleCreator.Create("T1", VehicleCreator.eVehicleType.Truck);

            Assert.Equal(200m, garage.CalculateBaseRate(truck));
        }

        [Fact]
        public void BaseRate_Motorcycle_Is75()
        {
            Garage garage = new Garage();
            Vehicle motorcycle = VehicleCreator.Create("M1", VehicleCreator.eVehicleType.FuelMotorcycle);

            Assert.Equal(75m, garage.CalculateBaseRate(motorcycle));
        }

        [Fact]
        public void Price_AddsLaborAt50PerHour()
        {
            Garage garage = TestData.GarageWithFixedCar("C1");
            garage.SetLaborHours("C1", 3f); // 3 * 50 = 150

            PriceBreakdown breakdown = garage.CalculatePrice("C1");

            Assert.Equal(150m, breakdown.LaborCost);
        }

        [Fact]
        public void Price_AddsPartsCost()
        {
            Garage garage = TestData.GarageWithFixedCar("C1");
            garage.AddPart("C1", new Part("brake pad", 75m, 2)); // 150

            PriceBreakdown breakdown = garage.CalculatePrice("C1");

            Assert.Equal(150m, breakdown.PartsCost);
        }

        [Fact]
        public void Price_AppliesEightPointFivePercentTax()
        {
            // Car base 100, no labor/parts/discount -> tax 8.50, total 108.50
            Garage garage = TestData.GarageWithFixedCar("C1");

            PriceBreakdown breakdown = garage.CalculatePrice("C1");

            Assert.Equal(100m, breakdown.Subtotal);
            Assert.Equal(8.50m, breakdown.Tax);
            Assert.Equal(108.50m, breakdown.Total);
        }

        [Fact]
        public void Price_SubtotalCombinesBaseLaborAndParts()
        {
            Garage garage = TestData.GarageWithFixedCar("C1");
            garage.SetLaborHours("C1", 2f);                  // 100
            garage.AddPart("C1", new Part("filter", 25m, 2)); // 50

            PriceBreakdown breakdown = garage.CalculatePrice("C1");

            Assert.Equal(250m, breakdown.Subtotal); // 100 + 100 + 50
        }

        [Fact]
        public void Price_ReturningCustomer_Gets10PercentOff()
        {
            // base 100 -> discount 10.00, taxable 90, tax 7.65, total 97.65
            Garage garage = TestData.GarageWithFixedCar("C1", i_Returning: true);

            PriceBreakdown breakdown = garage.CalculatePrice("C1");

            Assert.Equal(0.10m, breakdown.DiscountRate);
            Assert.Equal(10.00m, breakdown.DiscountAmount);
            Assert.Equal(97.65m, breakdown.Total);
        }

        [Fact]
        public void Price_NoDiscount_WhenNotReturningAndNotFleet()
        {
            Garage garage = TestData.GarageWithFixedCar("C1", i_Returning: false);

            PriceBreakdown breakdown = garage.CalculatePrice("C1");

            Assert.Equal(0m, breakdown.DiscountRate);
            Assert.Equal(0m, breakdown.DiscountAmount);
        }

        [Fact]
        public void Price_FleetAccount_FiveVehiclesSamePhone_Gets15PercentOff()
        {
            Garage garage = new Garage();
            for (int i = 0; i < 5; i++)
            {
                Vehicle car = VehicleCreator.Create("FL" + i, VehicleCreator.eVehicleType.FuelCar);
                garage.AddVehicle(car, "Fleet Co", "0529999999");
            }
            garage.UpdateVehicleStatus("FL0", Garage.eVehicleStatus.Fixed);

            PriceBreakdown breakdown = garage.CalculatePrice("FL0");

            Assert.Equal(0.15m, breakdown.DiscountRate);
        }

        [Fact]
        public void Price_Fleet_BeatsReturning_WhenBothApply()
        {
            Garage garage = new Garage();
            // 5 vehicles, the first flagged returning too -> fleet's 15% should win.
            garage.AddVehicle(VehicleCreator.Create("FL0", VehicleCreator.eVehicleType.FuelCar), "Fleet", "0521111111", true);
            for (int i = 1; i < 5; i++)
            {
                garage.AddVehicle(VehicleCreator.Create("FL" + i, VehicleCreator.eVehicleType.FuelCar), "Fleet", "0521111111");
            }
            garage.UpdateVehicleStatus("FL0", Garage.eVehicleStatus.Fixed);

            PriceBreakdown breakdown = garage.CalculatePrice("FL0");

            Assert.Equal(0.15m, breakdown.DiscountRate);
        }

        [Fact]
        public void Price_FourVehiclesSamePhone_IsNotFleet()
        {
            Garage garage = new Garage();
            for (int i = 0; i < 4; i++)
            {
                garage.AddVehicle(VehicleCreator.Create("Q" + i, VehicleCreator.eVehicleType.FuelCar), "Almost", "0523333333");
            }
            garage.UpdateVehicleStatus("Q0", Garage.eVehicleStatus.Fixed);

            PriceBreakdown breakdown = garage.CalculatePrice("Q0");

            Assert.Equal(0m, breakdown.DiscountRate); // 4 < 5 and not returning
        }
    }
}
