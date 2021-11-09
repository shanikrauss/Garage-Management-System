using System;
using System.Collections.Generic;
using System.Text;

namespace Ex03.GarageLogic
{
    //The class inherits from Vehicle class and consolidates fields and methods relevant to Truck and a fuel-driven vehicle.
    public class Truck : Vehicle
    {
        private const string k_Yes = "1";
        private const string k_No = "2";
        private const int k_WheelsAmount = 16;
        private const float k_WheelMaxAirPressure = 26;
        private const FuelEngine.eFuelType k_FuelType = FuelEngine.eFuelType.Soler;
        private const float k_MaxAmountFuelLiters = 120;
        private bool m_DrivesHazardousMaterials;
        private float m_MaxCarryingWeight;


        public Truck(string i_License) : base(i_License, k_WheelsAmount, k_WheelMaxAirPressure)
        {
            m_Engine = new FuelEngine(k_FuelType, k_MaxAmountFuelLiters);
        }


        public float MaxCarryingWeight
        {
            get
            {
                return m_MaxCarryingWeight;
            }
        }


        public bool DrivesHazardousMaterials
        {
            get
            {
                return m_DrivesHazardousMaterials;
            }
        }


        public override void UpdateInfo(List<string> i_VehicleArgsList)
        {
            base.UpdateInfo(i_VehicleArgsList);
            m_Engine.UpdateInfo(i_VehicleArgsList[3], k_MaxAmountFuelLiters);

            string drivesHazardousMaterials = i_VehicleArgsList[4];
            bool equalYes = drivesHazardousMaterials.Equals(k_Yes, StringComparison.CurrentCultureIgnoreCase);
            bool equalNo = drivesHazardousMaterials.Equals(k_No, StringComparison.CurrentCultureIgnoreCase);

            if (!equalYes && !equalNo)
            {
                throw new ArgumentException("Invalid 'drives Hazardous Materials'");
            }

            m_DrivesHazardousMaterials = equalYes ?  equalYes : equalNo;

            float maxCarryingWeight = 0;
            bool isValid = float.TryParse(i_VehicleArgsList[5], out maxCarryingWeight);

            if (!isValid)
            {
                throw new FormatException("Max carrying weight must be a positive number");
            }

            m_MaxCarryingWeight = maxCarryingWeight;
        }


        public override void GetArgsList(List<string> io_List)
        {
            base.GetArgsList(io_List);
            io_List.Add(string.Format("Drives Hazardous Materials (for yes - {0}, for no - {1}) ", k_Yes, k_No));
            io_List.Add("Max Carrying Weight");
        }


        public override string ToString()
        {
            string drivesHazardousMaterialsStr = m_DrivesHazardousMaterials ? "Yes" : "No";
            string vehicleArgsStr = base.ToString();
            string truckArgsStr = string.Format(@"Wheels Amount: {0}
 Wheel max air pressure: {1}
 Drives hazardous materials: {2}
 Max carrying weight: {3} {4}", k_WheelsAmount, k_WheelMaxAirPressure, drivesHazardousMaterialsStr, m_MaxCarryingWeight, Environment.NewLine);
            string mergedTruckArgsStr = string.Format("{0} {1}", vehicleArgsStr, truckArgsStr);

            return mergedTruckArgsStr;
        }
    }
}
