using System;
using System.Collections.Generic;
using Ex03.GarageLogic;
using Xunit;

namespace Ex03.RegressionTests
{
    // Guards pre-existing garage behavior against regressions introduced by the
    // payment work. Uses only types that exist at the base commit, so it compiles
    // and passes both before and after the solution (pass_to_pass).
    public class RegressionTests
    {
        [Fact]
        public void VehicleCreator_ListsAllFiveVehicleTypes()
        {
            List<string> types = VehicleCreator.GetVehicleList();

            Assert.Equal(5, types.Count);
            Assert.Contains("FuelCar", types);
            Assert.Contains("Truck", types);
            Assert.Contains("ElectricMotorcycle", types);
        }

        [Fact]
        public void VehicleCreator_CreatesRequestedConcreteType()
        {
            Vehicle truck = VehicleCreator.Create("T1", VehicleCreator.eVehicleType.Truck);
            Vehicle car = VehicleCreator.Create("C1", VehicleCreator.eVehicleType.ElectricCar);

            Assert.IsType<Truck>(truck);
            Assert.IsType<ElectricCar>(car);
            Assert.Equal("T1", truck.LicenseNumber);
        }

        [Fact]
        public void AddVehicle_ThenLicenseExists()
        {
            Garage garage = new Garage();
            Vehicle car = VehicleCreator.Create("C1", VehicleCreator.eVehicleType.FuelCar);

            garage.AddVehicle(car, "Bob", "0501234567");

            Assert.True(garage.CheckIfLicenseExist("C1"));
            Assert.False(garage.CheckIfLicenseExist("ZZZ"));
        }

        [Fact]
        public void GetVehicle_ReturnsTheAddedVehicle()
        {
            Garage garage = new Garage();
            Vehicle car = VehicleCreator.Create("C1", VehicleCreator.eVehicleType.FuelCar);
            garage.AddVehicle(car, "Bob", "0501234567");

            Assert.Same(car, garage.GetVehicle("C1"));
        }

        [Fact]
        public void GetVehicle_UnknownLicense_Throws()
        {
            Garage garage = new Garage();

            Assert.Throws<ArgumentException>(() => garage.GetVehicle("missing"));
        }

        [Fact]
        public void NewlyAddedVehicle_IsInProgress()
        {
            Garage garage = new Garage();
            Vehicle car = VehicleCreator.Create("C1", VehicleCreator.eVehicleType.FuelCar);
            garage.AddVehicle(car, "Bob", "0501234567");

            string inProgress = garage.GetLicenseListAccordingStatus(Garage.eVehicleStatus.InProgress).ToString();

            Assert.Contains("C1", inProgress);
        }

        [Fact]
        public void UpdateVehicleStatus_MovesVehicleBetweenStatusBuckets()
        {
            Garage garage = new Garage();
            Vehicle car = VehicleCreator.Create("C1", VehicleCreator.eVehicleType.FuelCar);
            garage.AddVehicle(car, "Bob", "0501234567");

            garage.UpdateVehicleStatus("C1", Garage.eVehicleStatus.Fixed);

            Assert.Contains("C1", garage.GetLicenseListAccordingStatus(Garage.eVehicleStatus.Fixed).ToString());
            Assert.DoesNotContain("C1", garage.GetLicenseListAccordingStatus(Garage.eVehicleStatus.InProgress).ToString());
        }
    }
}
