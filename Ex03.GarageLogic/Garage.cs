using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;


namespace Ex03.GarageLogic
{
    //The class incorporates the fields and methods responsible for the logical operation of the garage.
    //Contains an enum of status of the vehicles in the garage and a nested class called VehicleInGarage.
    //It has an array of VehicleInGarage class objects, representing the vehicles added to the garage.
    public class Garage
    {
        private readonly Dictionary<string, VehicleInGarage> r_VehiclesInGarage;


        //The nested class of the Garage class that contains fields and methods that combine and contain information
        //about the vehicle and its owner which entered the garage.
        private class VehicleInGarage
        {
            private Vehicle m_Vehicle;
            private readonly string r_OwnerName;
            private readonly string r_PhoneNumber;
            private eVehicleStatus m_Status;


            public VehicleInGarage(Vehicle i_Vehicle, string i_Name, string i_PhoneNumber)
            {
                m_Vehicle = i_Vehicle;
                r_OwnerName = i_Name;
                r_PhoneNumber = i_PhoneNumber;
                m_Status = eVehicleStatus.InProgress;
            }


            public string PhoneNumber
            {
                get
                {
                    return r_PhoneNumber;
                }
            }


            public string OwnerName
            {
                get
                {
                    return r_OwnerName;
                }
            }


            public Vehicle Vehicle
            {
                get
                {
                    return m_Vehicle;
                }
            }


            public eVehicleStatus Status
            {
                get
                {
                    return m_Status;
                }
                set
                {
                    if(!Enum.IsDefined(typeof(Garage.eVehicleStatus), value))
                    {
                        throw new ArgumentException("Invalid status");
                    }

                    m_Status = value;
                }
            }


            public override string ToString()
            {
                string garageArgsStr = string.Format(@" Owner name: {0}
 Phone number: {1}
 Status: {2}{3}", r_OwnerName, r_PhoneNumber, m_Status.ToString(), Environment.NewLine);
                string vehicleArgsStr = m_Vehicle.ToString();
                string mergedGarageArgsStr = string.Format("{0} {1}", garageArgsStr, vehicleArgsStr);

                return mergedGarageArgsStr;
            }
        }

        
        public enum eVehicleStatus
        {
            InProgress = 1,
            Fixed,
            Paid
        }


        public Garage()
        {
            r_VehiclesInGarage = new Dictionary<string, VehicleInGarage>();
        }


        private VehicleInGarage getVehicleInGarage(string i_License)
        {
            if(!CheckIfLicenseExist(i_License))
            {
                throw new ArgumentException("Vehicle doesn't exist in garage"); 
            }

            VehicleInGarage vehicleInGarage = r_VehiclesInGarage[i_License];

            return vehicleInGarage;
        }


        public bool CheckIfLicenseExist(string i_License)
        {
            bool exist = r_VehiclesInGarage.ContainsKey(i_License);

            return exist;
        }


        public Vehicle GetVehicle(string i_License)
        {
            Vehicle vehicle = getVehicleInGarage(i_License).Vehicle;

            return vehicle;
        }


        // The method assumes that the vehicle does not yet exist
        public void AddVehicle(Vehicle i_VehicleToAdd, string i_Name, string i_PhoneNumber)
        {
            VehicleInGarage vehicle = new VehicleInGarage(i_VehicleToAdd, i_Name, i_PhoneNumber);
            r_VehiclesInGarage.Add(i_VehicleToAdd.LicenseNumber, vehicle);
        }


        public void UpdateVehicleStatus(string i_License, eVehicleStatus i_NewStatus)
        {
            VehicleInGarage vehicle = getVehicleInGarage(i_License);
            
            vehicle.Status = i_NewStatus;
        }


        public void FillAirPressureToMax(string i_License)
        {
            Vehicle vehicle = GetVehicle(i_License);

            vehicle.InflateAllWheelToMax();
        }


        public void FillFuel(string i_License, FuelEngine.eFuelType i_FuelType, float i_AmountToAdd)
        {
            Vehicle vehicle = GetVehicle(i_License);
            FuelEngine engine = vehicle.Engine as FuelEngine;

            if(engine == null)
            {
                throw new ArgumentException("The vehicle is electric, cannot refuel");
            }

            engine.Refuel(i_FuelType, i_AmountToAdd);
        }


        public void ChargeBattery(string i_License, float i_HoursToAdd)
        {
            Vehicle vehicle = GetVehicle(i_License);
            ElectricEngine engine = vehicle.Engine as ElectricEngine;

            if(engine == null)
            {
                throw new ArgumentException("The vehicle has fuel engine, cannot charge");
            }

            engine.ChargeBattery(i_HoursToAdd);
        }


        public StringBuilder GetLicenseListOfAllVehicleInGarage()
        {
            StringBuilder LicenseListOfAllVehicleInGarage = new StringBuilder();

            foreach (eVehicleStatus status in Enum.GetValues(typeof(eVehicleStatus)))
            {
                LicenseListOfAllVehicleInGarage.AppendLine(GetLicenseListAccordingStatus(status).ToString());
            }

            return LicenseListOfAllVehicleInGarage;
        }


        public StringBuilder GetLicenseListAccordingStatus(eVehicleStatus i_Status)
        {
            StringBuilder LicenseListByStatus = new StringBuilder();

            foreach (VehicleInGarage vehicle in r_VehiclesInGarage.Values)
            {
                if(vehicle.Status == i_Status)
                {
                    LicenseListByStatus.AppendLine(vehicle.Vehicle.LicenseNumber);
                }
            }

            return LicenseListByStatus;
        }


        public string GetVehicleInfo(string i_License)
        {
            VehicleInGarage vehicle = getVehicleInGarage(i_License);
            string vehicleInfo = vehicle.ToString();

            return vehicleInfo;
        }


        public void GetRequestedArgsList(List<string> io_List)
        {
            io_List.Add("Owner name");
            io_List.Add("Phone number");
        }


        public void GetVehicleRequestedArgsList(Vehicle i_Vehicle, List<string> io_ArgsList)
        {
            i_Vehicle.GetArgsList(io_ArgsList);
        }


        public void UpdateVehicleInfo(Vehicle i_Vehicle, List<string> i_VehicleArgsList)
        {
            i_Vehicle.UpdateInfo(i_VehicleArgsList);
        }
    }
}
