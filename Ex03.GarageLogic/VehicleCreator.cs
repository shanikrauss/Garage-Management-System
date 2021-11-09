using System;
using System.Collections.Generic;

namespace Ex03.GarageLogic
{
    //The class manufactures vehicles and contains enum of the types of the vehicles available in the system.
    public abstract class VehicleCreator
    {
        public enum eVehicleType
        {
            FuelCar = 1,
            ElectricCar,
            FuelMotorcycle,
            ElectricMotorcycle,
            Truck
        }


        public static List<string> GetVehicleList()
        {
            List<string> vehicleList = new List<string>();

            foreach(string vehicleName in Enum.GetNames(typeof(eVehicleType)))
            {
                vehicleList.Add(vehicleName);
            }

            return vehicleList;
        }


        public static Vehicle Create(string i_License, eVehicleType i_VehicleType)
        {
            Vehicle vehicle = null;

            switch (i_VehicleType)
            {
                case eVehicleType.FuelCar:
                    vehicle = new FuelCar(i_License);
                    break;
                case eVehicleType.ElectricCar:
                    vehicle = new ElectricCar(i_License);
                    break;
                case eVehicleType.FuelMotorcycle:
                    vehicle = new FuelMotorcycle(i_License);
                    break;
                case eVehicleType.ElectricMotorcycle:
                    vehicle = new ElectricMotorcycle(i_License);
                    break;
                case eVehicleType.Truck:
                    vehicle = new Truck(i_License);
                    break;
                default:
                    throw new ArgumentException("Not Valid vehicle type");
                    break;
            }

            return vehicle;
        }
    }
}
